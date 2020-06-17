using System.Linq;

namespace GameServer.Utils
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
    }
}
