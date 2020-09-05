using System.Collections.Generic;
using GameJudge.Troops;
using GameJudge.Utils;

namespace GameJudge.WavesN
{
    public class WavesBuilder
    {
        private readonly Dictionary<int, List<Troop>> troopsForRound = new Dictionary<int, List<Troop>>();

        private int maxRedWave;
        private int maxBlueWave;

        public WavesBuilder Add(int round, int x, int y, PlayerSide player)
        {
            SetMaxWave(player, round);
            AddTroopToRound(round, new VectorTwo(x, y), player);
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

        private void AddTroopToRound(int round, VectorTwo position, PlayerSide player)
        {
            Troop troop = player == PlayerSide.Red ? TroopFactory.Red(position) : TroopFactory.Blue(position);

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
