using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace GameServer.Networking
{
    public static class Server
    {
        private static int _nextClientId;
        private static readonly Dictionary<int, Client> Clients = new Dictionary<int, Client>();

        public delegate Task PacketHandler(int fromClient, Packet packet);
        public static readonly Dictionary<int, PacketHandler> PacketHandlers= new Dictionary<int, PacketHandler>
        {
            {(int) ClientPackets.JoinGame, ServerHandle.JoinGame },
            {(int) ClientPackets.MoveTroop, ServerHandle.MoveTroop },
            {(int) ClientPackets.SendMessage, ServerHandle.SendMessage },
        };

        public static async Task ConnectNewClient(WebSocket socket)
        {
            Console.WriteLine("Connecting a new client");

            Client client = new Client(_nextClientId);

            Clients.Add(_nextClientId, client);
            _nextClientId++;

            await client.wsClient.Connect(socket);
        }

        public static async Task SendPacket(int toClient, Packet packet)
        {
            try
            {
                await Clients[toClient].wsClient.SendData(packet);
            }
            catch (WebSocketException ex)
            {
                Console.WriteLine("Exception thrown by server while sending data: " + ex);
                await GameHandler.ClientDisconnected(toClient);
            }
        }
    }
}
