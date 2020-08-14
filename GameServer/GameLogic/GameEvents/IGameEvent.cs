using GameServer.Networking;

namespace GameServer.GameLogic.ServerEvents
{
    public interface IGameEvent
    {
        public Packet GetPacket();
        public string GetString();
    }
}
