using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GameServer.Networking.Packets;
using GameServer.Utils;

namespace GameServer.Networking
{
    public class Client
    {
        private readonly int id;
        private readonly ServerSend sender;
        private readonly GameHandler gameHandler;
        private readonly ServerHandle serverHandle;
        private WebSocket socket;
        private bool isConnected;

        public Client(int id, ServerSend sender, GameHandler gameHandler, ServerHandle serverHandle)
        {
            this.id = id;
            this.sender = sender;
            this.gameHandler = gameHandler;
            this.serverHandle = serverHandle;
        }

        public async Task Connect(WebSocket socket)
        {
            this.socket = socket;
            isConnected = true;
            await sender.Welcome(id);
            while (isConnected) await BeginReceive();
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
                await gameHandler.ClientDisconnected(id);
                return;
            }
            
            if (data == null) return;

            ThreadManager.ExecuteOnMainThread(async () =>
            {
                using Packet packet = new Packet(data);
                serverHandle.Handle(id, packet);
            });
        }

        public async Task SendData(Packet packet)
        {
            ArraySegment<byte> buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(Serializer.Serialize(packet.ToArray())));
            await socket.SendAsync(buffer, WebSocketMessageType.Binary, true, CancellationToken.None);
        }
    }
}
