using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketServerAppTest.Utils;

namespace GameServer.Networking
{
    class Client
    {
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
                this.socket = socket;
                await ServerSend.Welcome(id, "Welcome to the server!");
                await BeginReceive();

            }

            private async Task<byte[]> Receive()
            {
                ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[4 * 1024]);
                var memoryStream = new MemoryStream();
                WebSocketReceiveResult result;

                do
                {
                    result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                    memoryStream.Write(buffer.Array, buffer.Offset, result.Count);
                } while (!result.EndOfMessage);

                memoryStream.Seek(0, SeekOrigin.Begin);

                if (result.MessageType != WebSocketMessageType.Close)
                {
                    using (var reader = new StreamReader(memoryStream, Encoding.UTF8))
                    {
                        string bytes = reader.ReadToEnd();
                        try
                        {
                            return Serializer.Deserialize(bytes);
                        }
                        catch
                        {
                            Console.WriteLine("Couldn't convert to bytes");
                        }
                    }
                }
                return null;
            }

            private async Task BeginReceive()
            {
                byte[] data = null;
                try
                {
                    data = await Receive();
                }
                catch (WebSocketException ex)
                {
                    Console.WriteLine(ex);
                    await GameHandler.ClientDisconnected(id);
                    return;
                }
                
                if (data == null)
                {
                    await BeginReceive();
                    return;
                }

                ThreadManager.ExecuteOnMainThread(async () =>
                {
                    using (Packet packet = new Packet(data))
                    {
                        int packetType = packet.ReadInt();
                        try
                        {
                            await Server.packetHandlers[packetType](id, packet);
                        }
                        catch (KeyNotFoundException)
                        {
                            Console.WriteLine("Unsupported packet type: " + packetType);
                        }
                    }
                });

                await BeginReceive();
            }

            public async Task SendData(Packet packet)
            {
                var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(Serializer.Serialize(packet.ToArray())));
                await socket.SendAsync(buffer, WebSocketMessageType.Binary, true, CancellationToken.None);
            }
        }
    }
}
