using System;

namespace GameServer.GameLogic.Battles
{
    public class StandardBattles : IBattleResolver
    {
        private static readonly Random Random = new Random();

        public BattleResult GetFightResult(Troop attacker, Troop defender)
        {
            BattleResult battleResult = new BattleResult();

            if (Random.Next(0, 6) < 3)
            {
                battleResult.DefenderDamaged = true;
            }
            if (defender.InControlZone(attacker.StartingPosition) && Random.Next(0, 6) < 3)
            {
                battleResult.AttackerDamaged = true;
            }

            return battleResult;
        }

        public BattleResult GetCollisionResult()
        {
            BattleResult battleResult = new BattleResult();

            if (Random.Next(0, 6) + Random.Next(0, 6) == 10)
            {
                battleResult.AttackerDamaged = true;
                battleResult.DefenderDamaged = true;
            }

            return battleResult;
        }
    }
}
