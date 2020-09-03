using GameServer.Networking;
using GameServer.Networking.Packets;

namespace GameServer.GameLogic.GameEvents
{
    public interface IGameEvent
    {
        public Packet GetPacket();
        public string GetString();
    }
}
