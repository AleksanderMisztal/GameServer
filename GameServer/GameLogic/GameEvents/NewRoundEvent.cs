using GameServer.Networking;
using System.Collections.Generic;
using System.Text;

namespace GameServer.GameLogic.ServerEvents
{
    public class NewRoundEvent : IGameEvent
    {
        private readonly List<Troop> troops;

        public NewRoundEvent(List<Troop> troops)
        {
            this.troops = troops;
        }

        public Packet GetPacket()
        {
            Packet packet = new Packet((int)ServerPackets.TroopSpawned);

            int length = troops.Count;
            packet.Write(length);

            for (int i = 0; i < length; i++)
                packet.Write(troops[i]);

            return packet;
        }

        public string GetString()
        {
            StringBuilder sb = new StringBuilder("New round event\n");
            foreach (var t in troops) sb.Append(t).Append("\n");
            return sb.ToString();
        }
    }
}
