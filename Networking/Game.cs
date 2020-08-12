using GameServer.GameLogic;

namespace GameServer.Networking
{
    public class Game
    {
        public int GameId { get; }
        public GameController Controller { get; }
        public int ClientRed { get; }
        public int ClientBlue { get; }

        public Game(int gameId, GameController controller, int clientRed, int clientBlue)
        {
            GameId = gameId;
            Controller = controller;
            ClientRed = clientRed;
            ClientBlue = clientBlue;
        }

        public override string ToString()
        {
            return $"(GameId : {GameId}, Blue : {ClientBlue}, Red : {ClientRed})";
        }
    }
}