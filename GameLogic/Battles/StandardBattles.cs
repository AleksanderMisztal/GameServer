using System;

namespace GameServer.GameLogic
{
    public class StandardBattles : IBattles
    {
        private static readonly Random random = new Random();

        public BattleResult GetFightResult(Troop attacker, Troop defender)
        {
            BattleResult battleResult = new BattleResult();

            if (random.Next(0, 6) < 3)
            {
                battleResult.DefenderDamaged = true;
            }
            if (defender.InControlZone(attacker.StartingPosition) && random.Next(0, 6) < 3)
            {
                battleResult.AttackerDamaged = true;
            }

            return battleResult;
        }

        public BattleResult GetCollisionResult()
        {
            BattleResult battleResult = new BattleResult();

            if (random.Next(0, 6) + random.Next(0, 6) == 10)
            {
                battleResult.AttackerDamaged = true;
                battleResult.DefenderDamaged = true;
            }

            return battleResult;
        }
    }
}
