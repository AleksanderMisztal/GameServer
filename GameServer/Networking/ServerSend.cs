using GameServer.GameLogic;
using GameServer.GameLogic.ServerEvents;
using System.Threading.Tasks;

namespace GameServer.Networking
{
    public static class ServerSend
    {
        public static async Task Welcome(int toClient)
        {
            using Packet packet = new Packet((int)ServerPackets.Welcome);
            await Server.SendPacket(toClient, packet);
        }

        public static async Task GameJoined(int toClient, string opponentName, PlayerSide side, Board board)
        {
            using Packet packet = new Packet((int)ServerPackets.GameJoined);

            packet.Write(opponentName);
            packet.Write((int)side);
            packet.Write(board);

            await Server.SendPacket(toClient, packet);
        }

        public static async Task GameEvent(int toClient, IGameEvent gameEvent)
        {
            using Packet packet = gameEvent.GetPacket();
            await Server.SendPacket(toClient, packet);
        }

        public static async Task MessageSent(int toClient, string message)
        {
            using Packet packet = new Packet((int)ServerPackets.MessageSent);
            packet.Write(message);
            await Server.SendPacket(toClient, packet);
        }

        public static async Task OpponentDisconnected(int toClient)
        {
            using Packet packet = new Packet((int)ServerPackets.OpponentDisconnected);
            await Server.SendPacket(toClient, packet);
        }
    }
}
