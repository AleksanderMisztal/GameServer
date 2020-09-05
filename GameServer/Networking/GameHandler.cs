using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameJudge;
using GameJudge.Areas;
using GameJudge.Utils;
using GameJudge.WavesN;

namespace GameServer.Networking
{
    public static class GameHandler
    {
        private static readonly Dictionary<int, Game> ClientToGame = new Dictionary<int, Game>();

        private static bool _someoneWaiting;
        private static User _waitingUser;

        public static async Task SendToGame(User newUser)
        {
            if (_someoneWaiting)
            {
                _someoneWaiting = false;
                Randomizer.RandomlyAssign(newUser, _waitingUser, out User redUser, out User blueUser);
                await InitializeNewGame(redUser, blueUser);
            }
            else
            {
                _someoneWaiting = true;
                _waitingUser = newUser;
            }
        }

        private static async Task InitializeNewGame(User playingRed, User playingBlue)
        {
            Waves waves = Waves.Test();
            Board board = Board.Test;
            GameController gc = CreateGameController(playingRed, playingBlue, waves, board);
            await InitializeGame(playingRed, playingBlue, gc, board);
            gc.BeginGame();
        }

        private static async Task InitializeGame(User playingRed, User playingBlue, GameController gc, Board board)
        {
            Game game = new Game(playingRed, playingBlue, gc);

            ClientToGame[playingRed.id] = game;
            ClientToGame[playingBlue.id] = game;

            await ServerSend.GameJoined(playingRed.id, playingBlue.name, PlayerSide.Red, board);
            await ServerSend.GameJoined(playingBlue.id, playingRed.name, PlayerSide.Blue, board);
        }

        private static GameController CreateGameController(User playingRed, User playingBlue, Waves waves, Board board)
        {
            GameController gc = new GameController(waves, board);
            gc.TroopsSpawned += async (sender, args) => await ServerSend.TroopsSpawned(playingRed.id, playingBlue.id, args);
            gc.TroopMoved += async (sender, args) => await ServerSend.TroopMoved(playingRed.id, playingBlue.id, args);
            gc.GameEnded += async (sender, args) => await ServerSend.GameEnded(playingRed.id, playingBlue.id, args);
            return gc;
        }

        public static void MoveTroop(int client, VectorTwo position, int direction)
        {
            if (direction < -1 || direction > 1)
            {
                Console.WriteLine($"Client {client} sent a move with illegal direction!");
                return;
            }
            Game game = ClientToGame[client];
            game.MakeMove(client, position, direction);
        }

        public static async Task SendMessage(int client, string message)
        {
            int opponent = GetOpponent(client);
            await ServerSend.MessageSent(opponent, message);
        }

        public static async Task ClientDisconnected(int client)
        {
            if (_someoneWaiting && client == _waitingUser.id)
                _someoneWaiting = false;

            int opponent = GetOpponent(client);

            ClientToGame.Remove(client);
            ClientToGame.Remove(opponent);

            await ServerSend.OpponentDisconnected(opponent);
        }

        private static int GetOpponent(int client)
        {
            Game game = ClientToGame[client];
            int opponentId = game.BlueUser.id ^ game.RedUser.id ^ client;
            return opponentId;
        }
    }
}