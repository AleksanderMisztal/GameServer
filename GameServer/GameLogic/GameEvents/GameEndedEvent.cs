using GameServer.Networking;
using GameServer.Networking.Packets;

namespace GameServer.GameLogic.GameEvents
{
    public class GameEndedEvent : IGameEvent
    {
        private readonly Score score;

        public GameEndedEvent(Score score)
        {
            this.score = score;
        }

        public Packet GetPacket()
        {
            Packet packet = new Packet((int)ServerPackets.GameEnded);

            packet.Write(score.Red);
            packet.Write(score.Blue);

            return packet;
        }

        public string GetString()
        {
            return $"Game ended event\nRed: {score.Red}, blue: {score.Blue}";
        }
    }
}
