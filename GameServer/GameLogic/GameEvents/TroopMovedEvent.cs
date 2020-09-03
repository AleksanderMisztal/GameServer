using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GameServer.GameLogic.Battles;
using GameServer.GameLogic.Utils;
using GameServer.Networking;
using GameServer.Networking.Packets;

namespace GameServer.GameLogic.GameEvents
{
    public class TroopMovedEvent : IGameEvent
    {
        private readonly Vector2Int position;
        private readonly int direction;
        private readonly List<BattleResult> battleResults;

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

            Debug.WriteLine("Battle count: " + battleResults.Count);
            packet.Write(battleResults.Count);
            foreach (BattleResult result in battleResults)
                packet.Write(result);

            return packet;
        }

        public string GetString()
        {
            string res = $"Troop moved event\n p: {position} d: {direction}\n";
            return battleResults.Aggregate(res, (current, b) => current + (b + "\n"));
        }
    }
}
