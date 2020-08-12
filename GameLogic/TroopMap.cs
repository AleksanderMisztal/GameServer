using GameServer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameServer.GameLogic
{
    public class TroopMap
    {
        private Dictionary<Vector2Int, Troop> map;

        public TroopMap()
        {
            map = new Dictionary<Vector2Int, Troop>();
        }

        public void AdjustPosition(Troop troop)
        {
            map.Remove(troop.StartingPosition);
            map.Add(troop.Position, troop);

            troop.StartingPosition = troop.Position;
        }

        public void Add(Troop troop)
        {
            map.Add(troop.Position, troop);
        }

        public Troop Get(Vector2Int position)
        {
            try
            {
                return map[position];
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }

        public void Remove(Troop troop)
        {
            map.Remove(troop.StartingPosition);
        }
    }
}
