using System.Collections.Generic;
using System.Diagnostics;
using GameServer.GameLogic;
using GameServer.GameLogic.Battles;
using GameServer.GameLogic.GameEvents;
using GameServer.GameLogic.Utils;
using GameServer.GameLogic.Waves;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GameServerTests
{
    [TestClass]
    public class GameControllerTests
    {
        private const int Forward = 0;

        private GameController gc;

        private void CreateGameController(Waves waves, int xMax, int yMax)
        {
            IBattleResolver battles = new AlwaysDamageBattles();
            Board board = new Board(xMax, yMax);

            gc = new GameController(battles, board, waves);
            gc.InitializeAndReturnEvent();
        }

        private List<IGameEvent> Move(PlayerSide player, int x, int y, int direction)
        {
            List<IGameEvent> events = gc.ProcessMove(player, new VectorTwo(x, y), direction);
            return events;
        }

        private void LogEvents(IEnumerable<IGameEvent> events)
        {
            foreach (IGameEvent ev in events) Trace.WriteLine(ev.GetString());
            Trace.WriteLine("");
        }


        [TestMethod]
        public void Should_EndGame_When_OneSideLosesAllTroops()
        {
            Waves waves = new WavesBuilder()
                .Add(1, 3, 3, PlayerSide.Blue)
                .Add(1, 2, 3, PlayerSide.Blue)
                .Add(1, 5, 3, PlayerSide.Red)
                .GetWaves();

            CreateGameController(waves, 10, 10);

            Move(PlayerSide.Blue, 3, 3, Forward);
            Move(PlayerSide.Blue, 4, 3, Forward);
            Move(PlayerSide.Blue, 6, 3, Forward);
            Move(PlayerSide.Blue, 2, 3, Forward);
            Move(PlayerSide.Blue, 3, 3, Forward);
            List<IGameEvent> events = Move(PlayerSide.Blue, 4, 3, Forward);

            Assert.AreEqual("Game ended event\nRed: 2, blue: 2", events[1].GetString());
        }

        [TestMethod]
        public void Should_ContinueGame_When_MoreTroopsWillSpawn()
        {
            Waves waves = new WavesBuilder()
                .Add(1, 3, 3, PlayerSide.Blue)
                .Add(1, 2, 3, PlayerSide.Blue)
                .Add(1, 5, 3, PlayerSide.Red)
                .Add(4, 1, 1, PlayerSide.Red)
                .GetWaves();

            CreateGameController(waves, 10, 10);

            Move(PlayerSide.Blue, 3, 3, Forward);
            Move(PlayerSide.Blue, 4, 3, Forward);
            Move(PlayerSide.Blue, 6, 3, Forward);
            Move(PlayerSide.Blue, 2, 3, Forward);
            Move(PlayerSide.Blue, 3, 3, Forward);
            List<IGameEvent> events = Move(PlayerSide.Blue, 4, 3, Forward);

            Assert.AreEqual(1, events.Count);
        }

        [TestMethod]
        public void Should_ControlTroopWithAI_When_ExitsBoard()
        {
            Waves waves = new WavesBuilder()
                .Add(1, 4, 3, PlayerSide.Blue)
                .Add(1, 5, 3, PlayerSide.Red)
                .GetWaves();

            CreateGameController(waves, 5, 5);

            List<IGameEvent> events = Move(PlayerSide.Blue, 4, 3, Forward);

            Assert.IsTrue(events.Count == 5);
        }

        [TestMethod]
        public void Should_AllowEnteringFriend_When_Blocked()
        {
            Waves waves = new WavesBuilder()
                .Add(1, 0, 3, PlayerSide.Blue)
                .Add(1, 1, 2, PlayerSide.Blue)
                .Add(1, 1, 3, PlayerSide.Blue)
                .Add(1, 1, 4, PlayerSide.Blue)
                .Add(1, 5, 3, PlayerSide.Red)
                .GetWaves();

            CreateGameController(waves, 10, 10);

            List<IGameEvent> events = Move(PlayerSide.Blue, 0, 3, Forward);

            Assert.AreEqual(events.Count, 1);
        }

        [TestMethod]
        public void Should_NotThrowIllegalMove_When_MovesAreLegal()
        {
            
        }
    }
}
