using GameServer.Networking;
using System;

namespace GameServer.GameLogic.ServerEvents
{
    public class LostOnTimeEvent : IServerEvent
    {
        private PlayerSide looser;

        public LostOnTimeEvent(PlayerSide looser)
        {
            this.looser = looser;
        }

        public Packet GetPacket()
        {
            Packet packet = new Packet((int)ServerPackets.LostOnTime);

            packet.Write((int)looser);

            return packet;
        }

        public string GetString()
        {
            return "Lost on time event";
        }
    }
}
