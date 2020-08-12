using GameServer.Networking;
using System.Collections.Generic;
using System.Text;

namespace GameServer.GameLogic.ServerEvents
{
    public class NewRoundEvent : IServerEvent
    {
        private List<TroopTemplate> troopTemplates;

        public NewRoundEvent(List<TroopTemplate> troopTemplates)
        {
            this.troopTemplates = troopTemplates;
        }

        public Packet GetPacket()
        {
            Packet packet = new Packet((int)ServerPackets.TroopSpawned);

            int length = troopTemplates.Count;
            packet.Write(length);

            for (int i = 0; i < length; i++)
                packet.Write(troopTemplates[i]);

            return packet;
        }

        public string GetString()
        {
            StringBuilder sb = new StringBuilder("New round event\n");
            foreach (var t in troopTemplates) sb.Append(t).Append("\n");
            return sb.ToString();
        }
    }
}
