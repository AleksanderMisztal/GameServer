using System.Collections.Generic;
using GameServer.Networking;
using GameServer.Utils;

namespace GameServer.GameLogic.ServerEvents
{
    public class TroopMovedEvent : IServerEvent
    {
        private Vector2Int position;
        private int direction;
        private List<BattleResult> battleResults;

        public TroopMovedEvent(Vector2Int position, int direction, List<BattleResult> battleResults)
        {
            this.position = position;
            this.direction = direction;
            this.battleResults = battleResults;
        }

        public Packet GetPacket()
        {
            Packet packet = new Packet((int)ServerPackets.TroopMoved);

            packet.Write(position);
            packet.Write(direction);

            packet.Write(battleResults.Count);
            for (int i = 0; i < battleResults.Count; i++)
                packet.Write(battleResults[i]);

            return packet;
        }

        public string GetString()
        {
            string res = $"Troop moved event\n p: {position} d: {direction}\n";
            foreach (var b in battleResults) res += b.ToString() + "\n";
            return res;
        }
    }
}
