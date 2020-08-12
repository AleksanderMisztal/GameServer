using GameServer.Utils;
using System.Collections.Generic;

namespace GameServer.GameLogic
{
    public class WavesBuilder
    {
        private Dictionary<int, List<TroopTemplate>> troopsForRound = new Dictionary<int, List<TroopTemplate>>();

        private int maxRedWave = 0;
        private int maxBlueWave = 0;

        private static readonly TroopTemplate blueTroop = new TroopTemplate(PlayerId.Blue, 5, 2, 0);
        private static readonly TroopTemplate redTroop = new TroopTemplate(PlayerId.Red, 5, 2, 3);

        public WavesBuilder Add(int round, int x, int y, PlayerId player)
        {
            SetMaxWave(player, round);
            AddTroopToRound(round, new Vector2Int(x, y), player);
            return this;
        }

        private void SetMaxWave(PlayerId player, int round)
        {
            if (player == PlayerId.Red)
                if (round > maxRedWave)
                    maxRedWave = round;
            if (player == PlayerId.Blue)
                if (round > maxBlueWave)
                    maxBlueWave = round;
        }

        private void AddTroopToRound(int round, Vector2Int position, PlayerId player)
        {
            var template = player == PlayerId.Red ? redTroop : blueTroop;
            var positionedTemplate = template.Deploy(position);

            try
            {
                troopsForRound[round].Add(positionedTemplate);
            }
            catch (KeyNotFoundException)
            {
                troopsForRound[round] = new List<TroopTemplate>();
                troopsForRound[round].Add(positionedTemplate);
            }
        }

        public Waves GetWaves()
        {
            return new Waves(troopsForRound, maxRedWave, maxBlueWave);
        }
    }
}
