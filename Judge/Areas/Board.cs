using GameJudge.Utils;

namespace GameJudge.Areas
{
    public class Board
    {
        public readonly int XMax;
        public readonly int YMax;

        public VectorTwo Center { get; }

        public Board(int xMax, int yMax)
        {
            this.XMax = xMax;
            this.YMax = yMax;

            Center = new VectorTwo(xMax / 2, yMax / 2);
        }

        public bool IsInside(VectorTwo p)
        {
            return p.X >= 0 && p.X <= XMax && p.Y >= 0 && p.Y <= YMax;
        }

        public static readonly Board Standard = new Board(20, 12);
        public static readonly Board Test = new Board(12, 7);
    }
}
