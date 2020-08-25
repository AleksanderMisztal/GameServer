using System.Linq;

namespace GameServer.GameLogic.Utils
{
    public static class Hex
    {
        public static Vector2Int GetAdjacentHex(Vector2Int cell, int direction)
        {
            return new HexOffset(cell).GetAdjacentHex(direction).ToVector();
        }

        public static Vector2Int[] GetNeighbours(Vector2Int cell)
        {
            return new HexOffset(cell).GetNeighbors().Select(c => c.ToVector()).ToArray();
        }

        public static Vector2Int[] GetControlZone(Vector2Int cell, int orientation)
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
