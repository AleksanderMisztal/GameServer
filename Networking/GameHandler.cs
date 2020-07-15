using System;
using System.Collections.Generic;
using GameServer.Utils;
using GameServer.GameLogic;
using System.Threading.Tasks;

namespace GameServer.Networking
{
    public class Game
    {
        public GameController Controller { get; }
        public int ClientRed { get; }
        public int ClientBlue { get; }


        public Game(GameController controller, int clientRed, int clientBlue)
        {
            Controller = controller;
            ClientRed = clientRed;
            ClientBlue = clientBlue;
        }

        public override string ToString()
        {
            return $"(GameId : {Controller.gameId}, Blue : {ClientBlue}, Red : {ClientRed})";
        }
    }

    public static class GameHandler
    {
        private static readonly Dictionary<int, Game> games = new Dictionary<int, Game>();
        private static readonly Dictionary<int, Game> clientToGame = new Dictionary<int, Game>();
        private static readonly Dictionary<int, PlayerId> clientToColor = new Dictionary<int, PlayerId>();
        private static readonly Dictionary<int, string> clientToUsername = new Dictionary<int, string>();

        private static int nextGameId = 0;
        private static bool someoneWaiting = false;
        private static int waitingClient;

        // Game -> Server
        public static async Task SendToGame(int client)
        {
            if (!someoneWaiting)
            {
                someoneWaiting = true;
                waitingClient = client;
                return;
            }

            someoneWaiting = false;

            int playingRed;
            int playingBlue;

            if (new Random().Next(2) == 0)
            {
                playingRed = waitingClient;
                playingBlue = client;
            }
            else
            {
                playingRed = client;
                playingBlue = waitingClient;
            }
            Game game = new Game(new GameController(nextGameId), playingRed, playingBlue);
            BoardParams board = game.Controller.Board;
            games.Add(nextGameId, game);

            clientToGame[playingRed] = game;
            clientToGame[playingBlue] = game;

            clientToColor[playingBlue] = PlayerId.Blue;
            clientToColor[playingRed] = PlayerId.Red;

            await game.Controller.Initialize();

            await ServerSend.GameJoined(playingBlue, clientToUsername[playingRed], PlayerId.Blue, board);
            await ServerSend.GameJoined(playingRed, clientToUsername[playingBlue], PlayerId.Red, board);

            nextGameId++;
        }

        public static async Task MoveTroop(int client, Vector2Int position, int direction)
        {
            if (!clientToGame.TryGetValue(client, out Game game))
            {
                return;
            }

            PlayerId color = clientToColor[client];
            try
            {
                await game.Controller.MoveTroop(color, position, direction);
            }
            catch (IllegalMoveException ex)
            {
                Console.WriteLine($"Illegal move: {ex.Message}");
            }
        }

        public static void AddToLobby(int client, string username)
        {
            clientToUsername[client] = username;
        }

        public static async Task SendMessage(int client, string message)
        {
            Game game;
            try
            {
                game = clientToGame[client];
            }
            catch 
            {
                await ServerSend.OpponentDisconnected(client);
                return;
            }
            int oponent = game.ClientBlue ^ game.ClientRed ^ client;
            await ServerSend.MessageSent(oponent, message);
        }


        // Server -> Game
        public static async Task TroopsSpawned(int gameId, List<TroopTemplate> templates)
        {
            if (!games.TryGetValue(gameId, out Game game))
            {
                return;
            }

            await ServerSend.TroopsSpawned(game.ClientBlue, templates);
            await ServerSend.TroopsSpawned(game.ClientRed, templates);
        }

        public static async Task TroopMoved(int gameId, Vector2Int position, int direction, List<BattleResult> battleResults)
        {
            Game game = games[gameId];

            await ServerSend.TroopMoved(game.ClientBlue, position, direction, battleResults);
            await ServerSend.TroopMoved(game.ClientRed, position, direction, battleResults);
        }

        public static async Task GameEnded(int gameId, int redScore, int blueScore)
        {
            Game game = games[gameId];

            await ServerSend.GameEnded(game.ClientBlue, redScore, blueScore);
            await ServerSend.GameEnded(game.ClientRed, redScore, blueScore);
        }


        // For displaying status
        public static Dictionary<int, string> Clients => clientToUsername;
        public static Dictionary<int, Game> Games => games;

        // Internal
        public static async Task ClientDisconnected(int clientId)
        {
            if (someoneWaiting && clientId == waitingClient)
            {
                someoneWaiting = false;
            }
            if (clientToGame.TryGetValue(clientId, out Game game))
            {
                int oponent = clientId ^ game.ClientBlue ^ game.ClientRed;
                await ServerSend.OpponentDisconnected(oponent);
            }
        }
    }
}