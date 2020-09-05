using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameJudge;
using GameJudge.Areas;
using GameJudge.Utils;
using GameJudge.WavesN;

namespace GameServer.Networking
{
    public class GameHandler
    {
        private readonly ServerSend sender;

        public GameHandler(ServerSend sender)
        {
            this.sender = sender;
        }

        private readonly Dictionary<int, Game> clientToGame = new Dictionary<int, Game>();

        private bool someoneWaiting;
        private User waitingUser;

        public async Task SendToGame(User newUser)
        {
            if (someoneWaiting)
            {
                someoneWaiting = false;
                Randomizer.RandomlyAssign(newUser, waitingUser, out User redUser, out User blueUser);
                await InitializeNewGame(redUser, blueUser);
            }
            else
            {
                someoneWaiting = true;
                waitingUser = newUser;
            }
        }

        private async Task InitializeNewGame(User playingRed, User playingBlue)
        {
            Waves waves = Waves.Test();
            Board board = Board.Test;
            GameController gc = CreateGameController(playingRed, playingBlue, waves, board);
            await InitializeGame(playingRed, playingBlue, gc, board);
            gc.BeginGame();
        }

        private async Task InitializeGame(User playingRed, User playingBlue, GameController gc, Board board)
        {
            Game game = new Game(playingRed, playingBlue, gc);

            clientToGame[playingRed.id] = game;
            clientToGame[playingBlue.id] = game;

            await sender.GameJoined(playingRed.id, playingBlue.name, PlayerSide.Red, board);
            await sender.GameJoined(playingBlue.id, playingRed.name, PlayerSide.Blue, board);
        }

        private GameController CreateGameController(User playingRed, User playingBlue, Waves waves, Board board)
        {
            GameController gc = new GameController(waves, board);
            gc.TroopsSpawned += async (emitter, args) => await sender.TroopsSpawned(playingRed.id, playingBlue.id, args);
            gc.TroopMoved += async (emitter, args) => await sender.TroopMoved(playingRed.id, playingBlue.id, args);
            gc.GameEnded += async (emitter, args) => await sender.GameEnded(playingRed.id, playingBlue.id, args);
            return gc;
        }

        public void MoveTroop(int client, VectorTwo position, int direction)
        {
            if (direction < -1 || direction > 1)
            {
                Console.WriteLine($"Client {client} sent a move with illegal direction!");
                return;
            }
            Game game = clientToGame[client];
            game.MakeMove(client, position, direction);
        }

        public async Task SendMessage(int client, string message)
        {
            int opponent = GetOpponent(client);
            await sender.MessageSent(opponent, message);
        }

        public async Task ClientDisconnected(int client)
        {
            if (someoneWaiting && client == waitingUser.id)
                someoneWaiting = false;

            int opponent = GetOpponent(client);

            clientToGame.Remove(client);
            clientToGame.Remove(opponent);

            await sender.OpponentDisconnected(opponent);
        }

        private int GetOpponent(int client)
        {
            Game game = clientToGame[client];
            int opponentId = game.BlueUser.id ^ game.RedUser.id ^ client;
            return opponentId;
        }
    }
}