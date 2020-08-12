using GameServer.Networking;

namespace GameServer.GameLogic.ServerEvents
{
    public class GameEndedEvent : IServerEvent
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
