namespace GameServer.GameLogic
{
    public enum PlayerId
    {
        Blue = 0,
        Red = 1
    }

    static class PlayerIdExtensions
    {
        public static PlayerId Opponent(this PlayerId player)
        {
            return player == PlayerId.Red ? PlayerId.Blue : PlayerId.Red;
        }
    }
}