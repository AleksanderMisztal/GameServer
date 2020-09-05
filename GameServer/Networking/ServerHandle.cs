using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameJudge.Utils;
using GameServer.Networking.Packets;

namespace GameServer.Networking
{
    public class ServerHandle
    {
        private delegate Task PacketHandler(int fromClient, Packet packet);
        
        private readonly GameHandler gameHandler;
        private readonly Dictionary<int, PacketHandler> packetHandlers;

        public ServerHandle(GameHandler gameHandler)
        {
            this.gameHandler = gameHandler;
            packetHandlers = new Dictionary<int, PacketHandler>
            {
                {(int) ClientPackets.JoinGame, JoinGame },
                {(int) ClientPackets.MoveTroop, MoveTroop },
                {(int) ClientPackets.SendMessage, SendMessage },
            };
        }

        public async Task Handle(int fromClient, Packet packet)
        {
            int packetType = packet.ReadInt();
            try
            {
                await packetHandlers[packetType](fromClient, packet);
            }
            catch (KeyNotFoundException)
            {
                Console.WriteLine("Unsupported packet type: " + packetType);
            }
        }        
        
        
        private async Task JoinGame(int fromClient, Packet packet)
        {
            string username = packet.ReadString();
            User newUser = new User(fromClient, username);
            await gameHandler.SendToGame(newUser);
        }

        private Task MoveTroop(int fromClient, Packet packet)
        {
            VectorTwo position = packet.ReadVector2Int();
            int direction = packet.ReadInt();

            gameHandler.MoveTroop(fromClient, position, direction);
            return Task.CompletedTask;
        }

        private async Task SendMessage(int fromClient, Packet packet)
        {
            string message = packet.ReadString();

            await gameHandler.SendMessage(fromClient, message);
        }
    }
}
