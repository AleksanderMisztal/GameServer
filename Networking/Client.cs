using System;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer.Networking
{
    class Client
    {
        public static int dataBufferSize = 4096;

        public int id;
        public WsClient wsClient;

        public Client(int _clientId)
        {
            id = _clientId;
            wsClient = new WsClient(id);
        }

        public class WsClient
        {
            private readonly int id;
            public WebSocket socket;

            public WsClient(int id)
            {
                this.id = id;
            }

            public async Task Connect(WebSocket socket)
            {
                Console.WriteLine("Connecting...");
                this.socket = socket;
                await ServerSend.Welcome(id, "Welcome to the server!");
                await BeginReceive();

            }

            private async Task<byte[]> Receive()
            {
                Console.WriteLine("Receiving...");
                ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[4 * 1024]);
                var memoryStream = new MemoryStream();
                WebSocketReceiveResult result;

                do
                {
                    result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                    memoryStream.Write(buffer.Array, buffer.Offset, result.Count);
                } while (!result.EndOfMessage);

                memoryStream.Seek(0, SeekOrigin.Begin);

                if (result.MessageType == WebSocketMessageType.Binary)
                {
                    return memoryStream.ToArray();
                }
                return null;
            }

            private async Task BeginReceive()
            {
                byte[] data = await Receive();

                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet packet = new Packet(data))
                    {
                        int packetType = packet.ReadInt();
                        Server.packetHandlers[packetType](id, packet);
                    }
                });

                await BeginReceive();
            }

            public async Task SendData(Packet packet)
            {
                Console.WriteLine("Sending data...");
                var buffer = new ArraySegment<byte>(packet.ToArray());
                await socket.SendAsync(buffer, WebSocketMessageType.Binary, true, CancellationToken.None);
            }
        }
    }
}
