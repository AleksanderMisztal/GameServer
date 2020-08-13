using GameServer.Networking;
using System;

namespace GameServer.GameLogic.ServerEvents
{
    public class GameJoinedEvent : IServerEvent
    {
        private string opponentName;
        private PlayerSide side;
        private Board board;

        public GameJoinedEvent(string opponentName, PlayerSide side, Board board)
        {
            this.opponentName = opponentName;
            this.side = side;
            this.board = board;
        }

        public Packet GetPacket()
        {
            Packet packet = new Packet((int)ServerPackets.GameJoined);

            packet.Write(opponentName);
            packet.Write((int)side);
            packet.Write(board);

            return packet;
        }

        public string GetString()
        {
            return "Game joined event";
        }
    }
}
