using GameServer.Utils;
using System.Collections.Generic;

namespace GameServer.GameLogic
{
    public class TroopMap
    {
        private readonly Dictionary<Vector2Int, Troop> map;

        private readonly HashSet<Troop> redTroops = new HashSet<Troop>();
        private readonly HashSet<Troop> blueTroops = new HashSet<Troop>();

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

        public HashSet<Troop> GetTroops(PlayerId player)
        {
            return player == PlayerId.Red ? redTroops : blueTroops;
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
            GetTroops(troop.Player).Remove(troop);
        }

        // TODO: Change from "dfs" to iterative bfs
        // TODO: Don't return cells outside the board
        public Vector2Int GetEmptyCell(Vector2Int seedPosition)
        {
            if (Get(seedPosition) == null) return seedPosition;

            Vector2Int[] neighbours = Hex.GetNeighbours(seedPosition);
            Randomizer.Randomize(neighbours);

            foreach (var position in neighbours)
            {
                if (Get(position) == null)
                {
                    return position;
                }
            }
            return GetEmptyCell(neighbours[0]);
        }

        public List<TroopTemplate> SpawnWave(List<TroopTemplate> wave)
        {
            foreach (var template in wave)
            {
                template.position = GetEmptyCell(template.position);
                Troop troop = new Troop(template);

                Add(troop);
            }
            return wave;
        }

        private void Add(Troop troop)
        {
            map.Add(troop.Position, troop);
            GetTroops(troop.Player).Add(troop);
        }
    }
}
