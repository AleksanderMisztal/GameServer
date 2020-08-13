using GameServer.Utils;
using System.Collections.Generic;

namespace GameServer.GameLogic
{
    public class WavesBuilder
    {
        private readonly Dictionary<int, List<Troop>> troopsForRound = new Dictionary<int, List<Troop>>();

        private int maxRedWave = 0;
        private int maxBlueWave = 0;

        public WavesBuilder Add(int round, int x, int y, PlayerSide player)
        {
            SetMaxWave(player, round);
            AddTroopToRound(round, new Vector2Int(x, y), player);
            return this;
        }

        private void SetMaxWave(PlayerSide player, int round)
        {
            if (player == PlayerSide.Red)
                if (round > maxRedWave)
                    maxRedWave = round;
            if (player == PlayerSide.Blue)
                if (round > maxBlueWave)
                    maxBlueWave = round;
        }

        private void AddTroopToRound(int round, Vector2Int position, PlayerSide player)
        {
            Troop troop = player == PlayerSide.Red ? Troop.Red(position) : Troop.Blue(position);

            try
            {
                troopsForRound[round].Add(troop);
            }
            catch (KeyNotFoundException)
            {
                troopsForRound[round] = new List<Troop>();
                troopsForRound[round].Add(troop);
            }
        }

        public Waves GetWaves()
        {
            return new Waves(troopsForRound, maxRedWave, maxBlueWave);
        }
    }
}
