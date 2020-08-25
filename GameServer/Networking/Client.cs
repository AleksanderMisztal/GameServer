using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer.Networking
{
    public class Client
    {
        public readonly WsClient wsClient;

        public Client(int clientId)
        {
            wsClient = new WsClient(clientId);
        }

        public class WsClient
        {
            private readonly int id;
            private WebSocket socket;
            private bool isConnected;

            public WsClient(int id)
            {
                this.id = id;
            }

            public async Task Connect(WebSocket socket)
            {
                this.socket = socket;
                isConnected = true;
                await ServerSend.Welcome(id);
                while (isConnected)
                {
                    await BeginReceive();
                }
            }

            private async Task<byte[]> Receive()
            {
                ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[4 * 1024]);
                MemoryStream memoryStream = new MemoryStream();
                WebSocketReceiveResult result;

                do
                {
                    result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                    memoryStream.Write(buffer.Array, buffer.Offset, result.Count);
                } while (!result.EndOfMessage);

                memoryStream.Seek(0, SeekOrigin.Begin);

                if (result.MessageType == WebSocketMessageType.Close) return null;
                using StreamReader reader = new StreamReader(memoryStream, Encoding.UTF8);
                string bytes = reader.ReadToEnd();
                try
                {
                    return Serializer.Deserialize(bytes);
                }
                catch
                {
                    Console.WriteLine("Couldn't convert to bytes");
                }
                return null;
            }

            private async Task BeginReceive()
            {
                byte[] data;
                try
                {
                    data = await Receive();
                }
                catch (WebSocketException ex)
                {
                    Console.WriteLine($"Exception occured: {ex}. Disconnecting client {id}.");
                    isConnected = false;
                    await GameHandler.ClientDisconnected(id);
                    return;
                }
                
                if (data == null)
                {
                    return;
                }

                ThreadManager.ExecuteOnMainThread(async () =>
                {
                    using Packet packet = new Packet(data);
                    int packetType = packet.ReadInt();
                    Console.WriteLine($"Received a packet of type {packetType}");
                    try
                    {
                        await Server.PacketHandlers[packetType](id, packet);
                    }
                    catch (KeyNotFoundException)
                    {
                        Console.WriteLine("Unsupported packet type: " + packetType);
                    }
                });
            }

            public async Task SendData(Packet packet)
            {
                ArraySegment<byte> buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(Serializer.Serialize(packet.ToArray())));
                await socket.SendAsync(buffer, WebSocketMessageType.Binary, true, CancellationToken.None);
            }
        }
    }
}
