using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace GameServer
{
    class Server
    {
        private static int nextClientId = 0;
        public static Dictionary<int, Client> clients = new Dictionary<int, Client>();

        public delegate void PacketHandler(int _fromClient, Packet _packet);
        public static Dictionary<int, PacketHandler> packetHandlers;

        public static void Start()
        {
            InitializePacketHandlers();
        }

        public static void Stop()
        {
            throw new NotImplementedException();
        }

        public static void InitializePacketHandlers()
        {
            packetHandlers = new Dictionary<int, PacketHandler>
            {
                {(int) ClientPackets.JoinLobby, ServerHandle.JoinLobby },
                {(int) ClientPackets.JoinGame, ServerHandle.JoinGame },
                {(int) ClientPackets.MoveTroop, ServerHandle.MoveTroop },
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
    }
}
