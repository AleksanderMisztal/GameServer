using GameServer.Utils;

namespace GameServer.GameLogic
{
    public class Board
    {
        // TODO: Only two params, always start at 0,0

        public readonly int xMin;
        public readonly int xMax;
        public readonly int yMax;
        public readonly int yMin;

        public Vector2Int Center { get; private set; }

        public Board(int xMin, int xMax, int yMin, int yMax)
        {
            this.xMin = xMin;
            this.xMax = xMax;
            this.yMin = yMin;
            this.yMax = yMax;

            int x = (xMax + xMin) / 2;
            int y = (yMax + yMin) / 2;

            Center = new Vector2Int(x, y);
        }

        public bool IsOutside(Vector2Int p)
        {
            return p.X < xMin || p.X > xMax || p.Y < yMin || p.Y > yMax;
        }

        public static readonly Board standard = new Board(0, 20, 0, 12);
        public static readonly Board test = new Board(0, 8, 0, 5);
    }
}
