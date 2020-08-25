using System;
using System.Linq;

namespace GameServer.GameLogic.Utils
{
    public class HexOffset
    {
        private static readonly Vector2Int[] EvenSteps = {
            new Vector2Int(1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(-1, 1),
            new Vector2Int(-1, 0),
            new Vector2Int(-1, -1),
            new Vector2Int(0, -1)
        };
        private static readonly Vector2Int[] OddSteps = {
            new Vector2Int(1, 0),
            new Vector2Int(1, 1),
            new Vector2Int(0, 1),
            new Vector2Int(-1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(1, -1)
        };

        private readonly int x;
        private readonly int y;


        private HexOffset(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public HexOffset(Vector2Int v)
        {
            x = v.X;
            y = v.Y;
        }


        public HexOffset GetAdjacentHex(int direction)
        {
            direction %= 6;
            while (direction < 0) direction += 6;
            Vector2Int[] steps = (y & 1) == 1 ? OddSteps : EvenSteps;
            Vector2Int step = steps[direction % 6];
            return new HexOffset(x + step.X, y + step.Y);
        }

        public HexOffset[] GetNeighbors()
        {
            Vector2Int[] steps = (y & 1) == 1 ? OddSteps : EvenSteps;
            return steps.Select(s => new HexOffset(x + s.X, y + s.Y)).ToArray();
        }

        public Vector2Int ToVector()
        {
            return new Vector2Int(x, y);
        }

        public override string ToString()
        {
            return "Offset(" + x + ", " + y + ")";
        }
        
        public override bool Equals(Object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType())) return false;
            HexOffset offset = (HexOffset)obj;
            return (x == offset.x) && (y == offset.y);
        }

        public override int GetHashCode()
        {
            return x ^ y;
        }
    }
}
