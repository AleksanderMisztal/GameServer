using System;

namespace GameJudge.GameEvents
{
    public class GameEndedEventArgs : EventArgs
    {
        public readonly Score Score;

        public GameEndedEventArgs(Score score)
        {
            Score = score;
        }

        public override string ToString()
        {
            return $"Game ended event\nRed: {Score.Red}, blue: {Score.Blue}";
        }
    }
}
