using GameJudge.Utils;

namespace GameJudge.Areas
{
    public interface IArea
    {
        bool IsInside(VectorTwo position);
    }
}