using System.Threading.Tasks;
using GameJudge;
using GameJudge.Areas;
using GameJudge.GameEvents;
using GameServer.Networking.Packets;

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

        public static async Task TroopsSpawned(int redId, int blueId, TroopsSpawnedEventArgs args)
        {
            using Packet packet = new Packet((int)ServerPackets.TroopSpawned);
            packet.Write(args);

            await Server.SendPacket(redId, packet);
            await Server.SendPacket(blueId, packet);
        }
    }
}
