using System;

namespace GameServer.GameLogic
{
    public class BattleResult
    {
        public bool DefenderDamaged { get; set; }
        public bool AttackerDamaged { get; set; }

        public BattleResult(bool defenderDamaged, bool attackerDamaged)
        {
            DefenderDamaged = defenderDamaged;
            AttackerDamaged = attackerDamaged;
        }

        public BattleResult() 
        {
            DefenderDamaged = false;
            AttackerDamaged = false;
        }
    }

    public class Battles
    {
        private static readonly Random random = new Random();

        public static BattleResult GetFightResult(Troop attacker, Troop defender)
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

        public static BattleResult GetCollisionResult()
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
