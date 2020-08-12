using GameServer.Networking;

namespace GameServer.GameLogic.ServerEvents
{
    public interface IServerEvent
    {
        public Packet GetPacket();
        public string GetString();
    }
}
