using System;
using System.Collections.Generic;
using GameServer.GameLogic.Utils;

namespace GameServer.GameLogic
{
    public class TroopMap
    {
        private readonly Dictionary<Vector2Int, Troop> map = new Dictionary<Vector2Int, Troop>();

        private readonly HashSet<Troop> redTroops = new HashSet<Troop>();
        private readonly HashSet<Troop> blueTroops = new HashSet<Troop>();
        private readonly Board board;

        public TroopMap(Board board)
        {
            this.board = board;
        }

        public void AdjustPosition(Troop troop)
        {
            map.Remove(troop.StartingPosition);
            map.Add(troop.Position, troop);

            troop.StartingPosition = troop.Position;
        }

        public HashSet<Troop> GetTroops(PlayerSide player)
        {
            return player == PlayerSide.Red ? redTroops : blueTroops;
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

        // TODO: Don't return cells outside the board
        private Vector2Int GetEmptyCell(Vector2Int seedPosition)
        {
            if (Get(seedPosition) == null) return seedPosition;

            Queue<Vector2Int> q = new Queue<Vector2Int>();
            q.Enqueue(seedPosition);
            while (q.Count > 0)
            {
                Vector2Int position = q.Dequeue();
                if (Get(position) == null) return position;
                Vector2Int[] neighbours = Hex.GetNeighbours(seedPosition);
                foreach (Vector2Int neigh in neighbours)
                    if (board.IsInside(neigh))
                        q.Enqueue(neigh);
            }
            throw new Exception("Couldn't find an empty cell");
        }

        public List<Troop> SpawnWave(List<Troop> wave)
        {
            foreach (Troop troop in wave)
            {
                troop.Position = GetEmptyCell(troop.Position);
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
