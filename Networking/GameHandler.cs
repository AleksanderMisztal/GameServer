using System.Collections.Generic;
using GameServer.Utils;
using GameServer.GameLogic;
using System.Threading.Tasks;
using GameServer.GameLogic.ServerEvents;

namespace GameServer.Networking
{
    public static class GameHandler
    {
        private static readonly Dictionary<int, Game> clientToGame = new Dictionary<int, Game>();

        private static bool someoneWaiting = false;
        private static User waitingClient;


        public static async Task SendToGame(int client, string username)
        {
            User newClient = new User(client, username);
            if (someoneWaiting)
            {
                someoneWaiting = false;
                Randomizer.RandomlyAssign(newClient, waitingClient, out User playingRed, out User playingBlue);
                await InitializeNewGame(playingRed, playingBlue);
            }
            else
            {
                someoneWaiting = true;
                waitingClient = newClient;
            }
        }

        private static async Task InitializeNewGame(User playingRed, User playingBlue)
        {
            Waves waves = Waves.Basic();
            Board board = Board.standard;
            Game game = new Game(new GameController(waves, board), playingRed, playingBlue);

            clientToGame[playingRed.id] = game;
            clientToGame[playingBlue.id] = game;

            await game.Initialize(board);
        }

        public static async Task MoveTroop(int client, Vector2Int position, int direction)
        {
            if (clientToGame.TryGetValue(client, out Game game))
            {
                List<IGameEvent> events = game.MakeMove(client, position, direction);
                foreach (var ev in events)
                {
                    await ServerSend.GameEvent(game.blueUser.id, ev);
                    await ServerSend.GameEvent(game.redUser.id, ev);
                }
            }
        }

        public static async Task SendMessage(int client, string message)
        {
            if (clientToGame.TryGetValue(client, out Game game))
            {
                int oponent = game.blueUser.id ^ game.redUser.id ^ client;
                await ServerSend.MessageSent(oponent, message);
            }
            else
            {
                await ServerSend.OpponentDisconnected(client);
            }
        }

        public static async Task ClientDisconnected(int clientId)
        {
            if (someoneWaiting && clientId == waitingClient.id)
                someoneWaiting = false;

            if (clientToGame.TryGetValue(clientId, out Game game))
            {
                int oponent = clientId ^ game.blueUser.id ^ game.redUser.id;

                clientToGame.Remove(clientId);
                clientToGame.Remove(oponent);

                await ServerSend.OpponentDisconnected(oponent);
            }
        }
    }
}