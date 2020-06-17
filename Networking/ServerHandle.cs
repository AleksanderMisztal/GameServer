using System;
using GameServer.Utils;

namespace GameServer
{
    class ServerHandle
    {
        public static void JoinLobby(int fromClient, Packet packet)
        {
            int clientIdCheck = packet.ReadInt();
            string username = packet.ReadString();

            Console.WriteLine($"Client with id {fromClient} joins with username {username}.");

            GameHandler.AddToLobby(fromClient, username);
        }

        public static void JoinGame(int fromClient, Packet packet)
        {
            int oponentId = packet.ReadInt();

            GameHandler.SendToGame(fromClient);
        }

        public static void MoveTroop(int fromClient, Packet packet)
        {
            Vector2Int position = packet.ReadVector2Int();
            int direction = packet.ReadInt();

            GameHandler.MoveTroop(fromClient, position, direction);
        }
    }
}
