using System;
using System.Collections.Generic;
using GameServer.Utils;
using GameServer.GameLogic;
using System.Diagnostics;

namespace GameServer
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
        public static void SendToGame(int client)
        {
            if (!someoneWaiting)
            {
                someoneWaiting = true;
                waitingClient = client;
                return;
            }

            someoneWaiting = false;

            Game game = new Game(new GameController(nextGameId), waitingClient, client);
            games.Add(nextGameId, game);

            clientToGame[client] = game;
            clientToGame[waitingClient] = game;

            clientToColor[client] = PlayerId.Blue;
            clientToColor[waitingClient] = PlayerId.Red;

            game.Controller.Initialize();

            ServerSend.GameJoined(client, clientToUsername[waitingClient], PlayerId.Blue);
            ServerSend.GameJoined(waitingClient, clientToUsername[client], PlayerId.Red);

            nextGameId++;
        }

        public static void MoveTroop(int client, Vector2Int position, int direction)
        {
            if (!clientToGame.TryGetValue(client, out Game game))
            {
                return;
            }

            PlayerId color = clientToColor[client];
            try
            {
                game.Controller.MoveTroop(color, position, direction);
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


        // Server -> Game
        public static void TroopsSpawned(int gameId, List<TroopTemplate> templates, List<Vector2Int> positions)
        {
            if (!games.TryGetValue(gameId, out Game game))
            {
                return;
            }

            ServerSend.TroopsSpawned(game.ClientBlue, templates, positions);
            ServerSend.TroopsSpawned(game.ClientRed, templates, positions);
        }

        public static void TroopMoved(int gameId, Vector2Int position, int direction, List<BattleResult> battleResults)
        {
            Game game = games[gameId];

            ServerSend.TroopMoved(game.ClientBlue, position, direction, battleResults);
            ServerSend.TroopMoved(game.ClientRed, position, direction, battleResults);
        }

        public static void GameEnded(int gameId, int redScore, int blueScore)
        {
            Game game = games[gameId];

            ServerSend.GameEnded(game.ClientBlue, redScore, blueScore);
            ServerSend.GameEnded(game.ClientRed, redScore, blueScore);
        }
    }
}