using System.Threading.Tasks;
using GameJudge;
using GameJudge.Areas;
using GameJudge.GameEvents;
using GameServer.Networking.Packets;

namespace GameServer.Networking
{
    public class ServerSend
    {
        private readonly Server server;

        public ServerSend(Server server)
        {
            this.server = server;
        }

        public async Task Welcome(int toClient)
        {
            using Packet packet = new Packet((int)ServerPackets.Welcome);
            await server.SendPacket(toClient, packet);
        }

        
        public async Task GameJoined(int toClient, string opponentName, PlayerSide side, Board board)
        {
            using Packet packet = new Packet((int)ServerPackets.GameJoined);

            packet.Write(opponentName);
            packet.Write((int)side);
            packet.Write(board);

            await server.SendPacket(toClient, packet);
        }

        public async Task MessageSent(int toClient, string message)
        {
            using Packet packet = new Packet((int)ServerPackets.MessageSent);
            packet.Write(message);
            await server.SendPacket(toClient, packet);
        }

        public async Task OpponentDisconnected(int toClient)
        {
            using Packet packet = new Packet((int)ServerPackets.OpponentDisconnected);
            await server.SendPacket(toClient, packet);
        }

        
        public async Task TroopsSpawned(int redId, int blueId, TroopsSpawnedEventArgs args)
        {
            using Packet packet = new Packet((int)ServerPackets.TroopSpawned);
            packet.Write(args.Troops);

            await server.SendPacket(redId, packet);
            await server.SendPacket(blueId, packet);
        }

        public async Task TroopMoved(int redId, int blueId, TroopMovedEventArgs args)
        {
            using Packet packet = new Packet((int)ServerPackets.TroopMoved);
            packet.Write(args.Position);
            packet.Write(args.Direction);
            packet.Write(args.BattleResults);

            await server.SendPacket(redId, packet);
            await server.SendPacket(blueId, packet);
        }

        public async Task GameEnded(int redId, int blueId, GameEndedEventArgs args)
        {
            using Packet packet = new Packet((int)ServerPackets.TroopMoved);
            packet.Write(args.Score.Red);
            packet.Write(args.Score.Blue);

            await server.SendPacket(redId, packet);
            await server.SendPacket(blueId, packet);
        }
    }
}
