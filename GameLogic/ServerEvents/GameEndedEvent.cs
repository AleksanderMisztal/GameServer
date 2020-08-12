using GameServer.Networking;

namespace GameServer.GameLogic.ServerEvents
{
    public class GameEndedEvent : IServerEvent
    {
        private int redScore;
        private int blueScore;

        public GameEndedEvent(int redScore, int blueScore)
        {
            this.redScore = redScore;
            this.blueScore = blueScore;
        }

        public Packet GetPacket()
        {
            Packet packet = new Packet((int)ServerPackets.GameEnded);

            packet.Write(redScore);
            packet.Write(blueScore);

            return packet;
        }

        public string GetString()
        {
            return $"Game ended event\nRed: {redScore}, blue: {blueScore}";
        }
    }
}
