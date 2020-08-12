using GameServer.Utils;

namespace GameServer.GameLogic
{
    public class Board
    {
        public readonly int xMin;
        public readonly int xMax;
        public readonly int yMax;
        public readonly int yMin;

        public Board(int xMin, int xMax, int yMin, int yMax)
        {
            this.xMin = xMin;
            this.xMax = xMax;
            this.yMin = yMin;
            this.yMax = yMax;
        }

        public Vector2Int GetCenter()
        {
            int x = (xMax + xMin) / 2;
            int y = (yMax + yMin) / 2;

            return new Vector2Int(x, y);
        }

        public static readonly Board standard = new Board(0, 20, 0, 12);
        public static readonly Board test = new Board(0, 8, 0, 5);
    }
}
