using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace GameServer.Networking
{
    class Server
    {
        private static int nextClientId = 0;
        public static Dictionary<int, Client> clients = new Dictionary<int, Client>();

        public delegate Task PacketHandler(int _fromClient, Packet _packet);
        public static Dictionary<int, PacketHandler> packetHandlers;

        public static void Start()
        {
            InitializePacketHandlers();
        }

        public static void Stop()
        {
            throw new NotImplementedException();
            //for client in clients client.disconnect()
        }

        public static void InitializePacketHandlers()
        {
            packetHandlers = new Dictionary<int, PacketHandler>
            {
                {(int) ClientPackets.JoinGame, ServerHandle.JoinGame },
                {(int) ClientPackets.MoveTroop, ServerHandle.MoveTroop },
                {(int) ClientPackets.SendMessage, ServerHandle.SendMessage },
            };
        }

        public static async Task ConnectNewClient(WebSocket socket)
        {
            Console.WriteLine("Connecting a new client");

            Client client = new Client(nextClientId);

            clients.Add(nextClientId, client);
            nextClientId++;

            await client.wsClient.Connect(socket);
        }

        public static async Task SendPacket(int toClient, Packet packet)
        {
            try
            {
                await clients[toClient].wsClient.SendData(packet);
            }
            catch (WebSocketException ex)
            {
                Console.WriteLine("Exception thrown by server while sending data: " + ex);
                await GameHandler.ClientDisconnected(toClient);
            }
        }
    }
}
