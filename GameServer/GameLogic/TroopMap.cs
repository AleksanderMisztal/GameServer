using System;
using System.Collections.Generic;
using System.Text;
using GameServer.GameLogic.Troops;
using GameServer.GameLogic.Utils;

namespace GameServer.GameLogic
{
    public class TroopMap
    {
        private readonly Dictionary<VectorTwo, Troop> map = new Dictionary<VectorTwo, Troop>();

        private readonly HashSet<Troop> redTroops = new HashSet<Troop>();
        private readonly HashSet<Troop> blueTroops = new HashSet<Troop>();
        private readonly Board board;

        public TroopMap(Board board)
        {
            this.board = board;
        }

        public void AdjustPosition(Troop troop, VectorTwo startingPosition)
        {
            map.Remove(startingPosition);
            map.Add(troop.Position, troop);
        }

        public HashSet<Troop> GetTroops(PlayerSide player)
        {
            return player == PlayerSide.Red ? redTroops : blueTroops;
        }

        public Troop Get(VectorTwo position)
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

        public void Remove(Troop troop, VectorTwo startingPosition)
        {
            map.Remove(startingPosition);
            GetTroops(troop.Player).Remove(troop);
        }

        // TODO: Don't return cells outside the board
        private VectorTwo GetEmptyCell(VectorTwo seedPosition)
        {
            if (Get(seedPosition) == null) return seedPosition;

            Queue<VectorTwo> q = new Queue<VectorTwo>();
            q.Enqueue(seedPosition);
            while (q.Count > 0)
            {
                VectorTwo position = q.Dequeue();
                if (Get(position) == null) return position;
                VectorTwo[] neighbours = Hex.GetNeighbours(seedPosition);
                foreach (VectorTwo neigh in neighbours)
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

        public void LogTroops()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Troop troop in map.Values) 
                sb.Append($"{troop.Position}, {troop.Player}\n");
            Console.WriteLine(sb);
        }
    }
}
