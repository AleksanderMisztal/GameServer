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

        public int StartTime { get; }
        public int BlueTime { get; private set; } = 60;
        public int RedTime { get; private set; } = 60;
        public int LastMoveTime { get; set; }
        public PlayerId LastMoving { get; set; } = PlayerId.Red;

        public async void OnMove(int timeStamp)
        {
            int elapsed = timeStamp - LastMoveTime;
            LastMoveTime = timeStamp;
            int timeLeft = LastMoving == PlayerId.Red ? BlueTime -= elapsed : RedTime -= elapsed;
            LastMoving = LastMoving == PlayerId.Red ? PlayerId.Blue : PlayerId.Red;

            await Task.Delay(timeLeft + 1);
            if (!Controller.gameEnded)
            {
                if (LastMoving == PlayerId.Red)
                {
                    int now = (int)DateTime.Now.TimeOfDay.TotalSeconds;
                    if (now - LastMoveTime > BlueTime)
                    {
                        await ServerSend.LostOnTime(ClientBlue, PlayerId.Blue);
                        await ServerSend.LostOnTime(ClientRed, PlayerId.Blue);
                    }
                }
                if (LastMoving == PlayerId.Blue)
                {
                    int now = (int)DateTime.Now.TimeOfDay.TotalSeconds;
                    if (now - LastMoveTime > RedTime)
                    {
                        await ServerSend.LostOnTime(ClientBlue, PlayerId.Red);
                        await ServerSend.LostOnTime(ClientRed, PlayerId.Red);
                    }
                }
            }
        }

        public Game(GameController controller, int clientRed, int clientBlue, int startTime)
        {
            Controller = controller;
            ClientRed = clientRed;
            ClientBlue = clientBlue;
            StartTime = startTime;
            LastMoveTime = startTime;
        }

        public override string ToString()
        {
            return $"(GameId : {Controller.gameId}, Blue : {ClientBlue}, Red : {ClientRed})";
        }
    }

    public static class GameHandler
    {
        public static readonly Dictionary<int, Game> games = new Dictionary<int, Game>();
        private static readonly Dictionary<int, Game> clientToGame = new Dictionary<int, Game>();
        private static readonly Dictionary<int, PlayerId> clientToColor = new Dictionary<int, PlayerId>();
        public static readonly Dictionary<int, string> clientToUsername = new Dictionary<int, string>();

        private static int nextGameId = 0;
        private static bool someoneWaiting = false;
        private static int waitingClient;

        // Game -> Server
        public static void AddToLobby(int client, string username)
        {
            clientToUsername[client] = username;
        }

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

            int startTime = (int)DateTime.Now.TimeOfDay.TotalSeconds;
            Game game = new Game(new GameController(nextGameId), playingRed, playingBlue, startTime);
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

            int timeStamp = (int)DateTime.Now.TimeOfDay.TotalSeconds - game.StartTime;
            game.OnMove(timeStamp);

            await ServerSend.TroopsSpawned(game.ClientBlue, timeStamp, templates);
            await ServerSend.TroopsSpawned(game.ClientRed, timeStamp, templates);
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

            games.Remove(gameId);
            clientToGame.Remove(game.ClientBlue);
            clientToGame.Remove(game.ClientRed);

            await ServerSend.GameEnded(game.ClientBlue, redScore, blueScore);
            await ServerSend.GameEnded(game.ClientRed, redScore, blueScore);
        }

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

                clientToGame.Remove(clientId);
                clientToGame.Remove(oponent);
                games.Remove(game.Controller.gameId);

                await ServerSend.OpponentDisconnected(oponent);
            }
        }
    }
}