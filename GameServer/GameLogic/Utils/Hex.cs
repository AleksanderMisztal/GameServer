using System.Linq;

namespace GameServer.GameLogic.Utils
{
    public class Hex
    {
        public static int GetDistance(Vector2Int v1, Vector2Int v2)
        {
            Vector3Int cube1 = new HexOffset(v1).ToCube().ToVector();
            Vector3Int cube2 = new HexOffset(v2).ToCube().ToVector();

            Vector3Int dif = (cube1 - cube2).Abs();

            return new[] { dif.X, dif.Y, dif.Z }.Max();
        }

        public static Vector2Int GetAdjacentHex(Vector2Int cell, int direction)
        {
            return new HexOffset(cell).GetAdjacentHex(direction).ToVector();
        }

        public static Vector2Int[] GetNeighbours(Vector2Int cell)
        {
            return new HexOffset(cell).GetNeighbors().Select(c => c.ToVector()).ToArray();
        }

        public static Vector2Int[] GetControllZone(Vector2Int cell, int orientation)
        {
            Vector2Int[] cells = new Vector2Int[3];
            for (int i = -1; i < 2; i++)
            {
                int direction = (orientation + i + 6) % 6;
                cells[i + 1] = GetAdjacentHex(cell, direction);
            }
            return cells;
        }
    }
}
