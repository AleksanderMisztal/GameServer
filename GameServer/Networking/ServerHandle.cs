using System.Threading.Tasks;
using GameServer.GameLogic.Utils;

namespace GameServer.Networking
{
    public static class ServerHandle
    {
        public static async Task JoinGame(int fromClient, Packet packet)
        {
            string username = packet.ReadString();
            User newUser = new User(fromClient, username);
            await GameHandler.SendToGame(newUser);
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
