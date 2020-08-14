using GameServer.GameLogic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GameServerTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Board board = new Board(5, 5);
            Assert.IsNotNull(board);
        }
    }
}
