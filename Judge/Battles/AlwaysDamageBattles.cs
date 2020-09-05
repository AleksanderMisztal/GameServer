using GameJudge.Troops;
using GameJudge.Utils;

namespace GameJudge.Battles
{
    public class AlwaysDamageBattles : IBattleResolver
    {
        public BattleResult GetFightResult(Troop defender, VectorTwo attackerPosition)
        {
            return new BattleResult(true, true);
        }

        public BattleResult GetCollisionResult()
        {
            return new BattleResult(true, true);
        }
    }
}
