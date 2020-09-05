using System.Diagnostics;
using GameJudge;
using GameJudge.Areas;
using GameJudge.WavesN;
using NUnit.Framework;

namespace JudgeTests
{
    public class GameControllerTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestInstantiation()
        {
            Waves waves = Waves.Basic();
            Board board = Board.Standard;
            
            GameController gc = new GameController(waves, board);

            gc.TroopMoved += (sender, args) => Debug.WriteLine(args);
            gc.TroopsSpawned += (sender, args) => Debug.WriteLine(args);
            gc.GameEnded += (sender, args) => Debug.WriteLine(args);
            
            gc.BeginGame();
        }
    }
}