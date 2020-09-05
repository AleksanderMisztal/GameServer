using GameJudge.Troops;
using GameJudge.Utils;

namespace GameJudge.Battles
{
    public interface IBattleResolver
    {
        BattleResult GetFightResult(Troop defender, VectorTwo attackerPosition);

        BattleResult GetCollisionResult();
    }
}
