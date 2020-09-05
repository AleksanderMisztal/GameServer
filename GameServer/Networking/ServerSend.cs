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
            packet.Write(args.Troops);

            await Server.SendPacket(redId, packet);
            await Server.SendPacket(blueId, packet);
        }

        public static async Task TroopMoved(int redId, int blueId, TroopMovedEventArgs args)
        {
            using Packet packet = new Packet((int)ServerPackets.TroopMoved);
            packet.Write(args.Position);
            packet.Write(args.Direction);
            packet.Write(args.BattleResults);

            await Server.SendPacket(redId, packet);
            await Server.SendPacket(blueId, packet);
        }

        public static async Task GameEnded(int redId, int blueId, GameEndedEventArgs args)
        {
            using Packet packet = new Packet((int)ServerPackets.TroopMoved);
            packet.Write(args.Score.Red);
            packet.Write(args.Score.Blue);

            await Server.SendPacket(redId, packet);
            await Server.SendPacket(blueId, packet);
        }
    }
}
