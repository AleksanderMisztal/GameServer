using System;
using System.Collections.Generic;
using GameServer.Utils;
using GameServer.GameLogic;
using System.Threading.Tasks;
using GameServer.GameLogic.ServerEvents;

namespace GameServer.Networking
{
    public static class GameHandler
    {
        public static readonly Dictionary<int, Game> games = new Dictionary<int, Game>();
        private static readonly Dictionary<int, Game> clientToGame = new Dictionary<int, Game>();
        private static readonly Dictionary<int, PlayerSide> clientToColor = new Dictionary<int, PlayerSide>();
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

            Waves waves = Waves.Basic();
            Board board = Board.standard;

            Game game = new Game(nextGameId++, new GameController(waves, board), playingRed, playingBlue);

            games.Add(nextGameId, game);

            clientToGame[playingRed] = game;
            clientToGame[playingBlue] = game;

            clientToColor[playingBlue] = PlayerSide.Blue;
            clientToColor[playingRed] = PlayerSide.Red;

            //TODO: should send game started

            var redGameJoined = new GameJoinedEvent(clientToUsername[playingBlue], PlayerSide.Red, board);
            var blueGameJoined = new GameJoinedEvent(clientToUsername[playingRed], PlayerSide.Blue, board);

            await ServerSend.SendEvent(playingRed, redGameJoined);
            await ServerSend.SendEvent(playingBlue, blueGameJoined);

            var ev = game.Controller.InitializeAndReturnEvent();

            await ServerSend.SendEvent(playingRed, ev);
            await ServerSend.SendEvent(playingBlue, ev);
        }

        public static async Task MoveTroop(int client, Vector2Int position, int direction)
        {
            if (clientToGame.TryGetValue(client, out Game game))
            {
                PlayerSide color = clientToColor[client];
                try
                {
                    List<IServerEvent> events = game.Controller.ProcessMove(color, position, direction);
                    foreach (var ev in events)
                    {
                        await ServerSend.SendEvent(game.ClientBlue, ev);
                        await ServerSend.SendEvent(game.ClientRed, ev);
                    }
                }
                catch (IllegalMoveException ex)
                {
                    Console.WriteLine($"Illegal move: {ex.Message}");
                }
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
                await ServerSend.SendEvent(client, new OpponentDisconnectedEvent());
                return;
            }
            int oponent = game.ClientBlue ^ game.ClientRed ^ client;
            await ServerSend.SendEvent(oponent, new MessageSentEvent(message));
        }


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
                games.Remove(game.GameId);

                await ServerSend.SendEvent(oponent, new OpponentDisconnectedEvent());
            }
        }
    }
}