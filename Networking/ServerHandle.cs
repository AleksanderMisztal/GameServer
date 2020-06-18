using System;
using System.Threading.Tasks;
using GameServer.Utils;

namespace GameServer.Networking
{
    class ServerHandle
    {
        public static async Task JoinLobby(int fromClient, Packet packet)
        {
            int clientIdCheck = packet.ReadInt();
            string username = packet.ReadString();

            Console.WriteLine($"Client with id {fromClient} joins with username {username}.");

            GameHandler.AddToLobby(fromClient, username);
        }

        public static async Task JoinGame(int fromClient, Packet packet)
        {
            int oponentId = packet.ReadInt();

            await GameHandler.SendToGame(fromClient);
        }

        public static async Task MoveTroop(int fromClient, Packet packet)
        {
            Console.WriteLine($"Player {fromClient} wants to move a troop.");
            Vector2Int position = packet.ReadVector2Int();
            int direction = packet.ReadInt();

            await GameHandler.MoveTroop(fromClient, position, direction);
        }
    }
}
