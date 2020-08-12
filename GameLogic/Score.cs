namespace GameServer.GameLogic
{
    public class Score
    {
        public int Red { get; private set; } = 0;
        public int Blue { get; private set; } = 0;

        public void Increment(PlayerId player)
        {
            if (player == PlayerId.Red) Red++;
            if (player == PlayerId.Blue) Blue++;
        }
    }
}
