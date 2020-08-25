using GameServer.Networking;

namespace GameServer.GameLogic.GameEvents
{
    public interface IGameEvent
    {
        public Packet GetPacket();
        public string GetString();
    }
}
