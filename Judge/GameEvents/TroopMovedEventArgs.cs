using System;
using System.Collections.Generic;
using System.Linq;
using GameJudge.Battles;
using GameJudge.Utils;

namespace GameJudge.GameEvents
{
    public class TroopMovedEventArgs : EventArgs
    {
        public readonly VectorTwo Position;
        public readonly int Direction;
        public readonly List<BattleResult> BattleResults;

        public TroopMovedEventArgs(VectorTwo position, int direction, List<BattleResult> battleResults)
        {
            Position = position;
            Direction = direction;
            BattleResults = battleResults;
        }

        public override string ToString()
        {
            string res = $"Troop moved event\n p: {Position} d: {Direction}\n";
            return BattleResults.Aggregate(res, (current, b) => current + (b + "\n"));
        }
    }
}
