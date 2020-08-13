using System;
using System.Threading.Tasks;
using GameServer.Utils;

namespace GameServer.Networking
{
    class ServerHandle
    {
        public static async Task JoinLobby(int fromClient, Packet packet)
        {
            string username = packet.ReadString();
            Console.WriteLine($"Client with id {fromClient} joins with username {username}.");
            GameHandler.AddToLobby(fromClient, username);
        }

        public static async Task JoinGame(int fromClient, Packet packet)
        {
            Console.WriteLine($"Client with id {fromClient} is being sent to game");
            await GameHandler.SendToGame(fromClient);
        }

        public static async Task MoveTroop(int fromClient, Packet packet)
        {
            Vector2Int position = packet.ReadVector2Int();
            int direction = packet.ReadInt();

            await GameHandler.MoveTroop(fromClient, position, direction);
        }

        public static async Task SendMessage(int fromClient, Packet packet)
        {
            string message = packet.ReadString();

            await GameHandler.SendMessage(fromClient, message);
        }
    }
}
