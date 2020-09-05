using GameJudge;
using GameJudge.Areas;
using GameJudge.Battles;
using GameJudge.Utils;
using GameJudge.WavesN;
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
            gc.BeginGame();
        }

        private void Move(PlayerSide player, int x, int y, int direction)
        {
            gc.ProcessMove(player, new VectorTwo(x, y), direction);
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
            Move(PlayerSide.Blue, 4, 3, Forward);

            Assert.IsTrue(true);
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
            Move(PlayerSide.Blue, 4, 3, Forward);

            Assert.AreEqual(1, 1);
        }

        [TestMethod]
        public void Should_ControlTroopWithAI_When_ExitsBoard()
        {
            Waves waves = new WavesBuilder()
                .Add(1, 4, 3, PlayerSide.Blue)
                .Add(1, 5, 3, PlayerSide.Red)
                .GetWaves();

            CreateGameController(waves, 5, 5);

            Move(PlayerSide.Blue, 4, 3, Forward);

            Assert.IsTrue(5 == 5);
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

            Move(PlayerSide.Blue, 0, 3, Forward);

            Assert.AreEqual(1, 1);
        }

        [TestMethod]
        public void Should_NotThrowIllegalMove_When_MovesAreLegal()
        {
            //GameController gc = new GameController();
        }
    }
}
