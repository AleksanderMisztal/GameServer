using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;
using GameServer.Networking.Packets;

namespace GameServer.Networking
{
    public class Server
    {
        private readonly ServerSend sender;
        private readonly GameHandler gameHandler;
        private readonly ServerHandle serverHandle;

        public Server()
        {
            sender = new ServerSend(this);
            gameHandler = new GameHandler(sender);
            serverHandle = new ServerHandle(gameHandler);
        }

        private int nextClientId;
        private readonly Dictionary<int, Client> clients = new Dictionary<int, Client>();


        public async Task ConnectNewClient(WebSocket socket)
        {
            Console.WriteLine("Connecting a new client");

            Client client = new Client(nextClientId, sender, gameHandler, serverHandle);

            clients.Add(nextClientId, client);
            nextClientId++;

            await client.Connect(socket);
        }

        public async Task SendPacket(int toClient, Packet packet)
        {
            try
            {
                await clients[toClient].SendData(packet);
            }
            catch (WebSocketException ex)
            {
                Console.WriteLine("Exception thrown by server while sending data: " + ex);
                await gameHandler.ClientDisconnected(toClient);
            }
        }
    }
}
