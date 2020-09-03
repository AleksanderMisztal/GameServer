using System;
using GameServer.GameLogic.Troops;
using GameServer.GameLogic.Utils;

namespace GameServer.GameLogic.Battles
{
    public class StandardBattles : IBattleResolver
    {
        private static readonly Random Random = new Random();

        public BattleResult GetFightResult(Troop defender, VectorTwo attackerPosition)
        {
            bool defenderDamaged = Random.Next(0, 6) < 3;
            bool attackerDamaged = defender.InControlZone(attackerPosition) && Random.Next(0, 6) < 3;

            return new BattleResult(defenderDamaged, attackerDamaged);
        }

        public BattleResult GetCollisionResult()
        {
            if (Random.Next(0, 6) + Random.Next(0, 6) != 10) return new BattleResult();
            return new BattleResult(true, true);
        }
    }
}
