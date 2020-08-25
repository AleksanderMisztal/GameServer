using GameServer.GameLogic;
using GameServer.GameLogic.Utils;
using GameServer.GameLogic.Waves;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GameServerTests
{
    [TestClass]
    public class MoveValidatorTests
    {
        private TroopMap troopMap;
        private WavesBuilder wb;
        private PlayerSide player0;
        private Board board;

        private MoveValidator validator;


        private void CreateValidator()
        {
            board = new Board(5, 5);
            troopMap = new TroopMap(board);
            player0 = PlayerSide.Red;
            wb = new WavesBuilder();

            validator = new MoveValidator(troopMap, board, player0);
            validator.ToggleActivePlayer();
        }

        private void AddTroop(int x, int y)
        {
            wb.Add(1, x, y, PlayerSide.Blue);
        }

        private void DoAddTroops()
        {
            Waves waves = wb.GetWaves();
            troopMap.SpawnWave(waves.GetTroops(1));
            OnTurnBegin(PlayerSide.Blue);
        }

        private void OnTurnBegin(PlayerSide player)
        {
            foreach (Troop troop in troopMap.GetTroops(player))
            {
                troop.ResetMovePoints();
            }
        }

        private Troop GetTroop(int x, int y)
        {
            return troopMap.Get(new Vector2Int(x, y));
        }

        [TestMethod]
        public void Should_ReturnTrue_When_NormalMove()
        {
            CreateValidator();
            AddTroop(2, 2);
            DoAddTroops();
            Troop troop = GetTroop(2, 2);

            Assert.IsTrue(validator.IsLegalMove(PlayerSide.Blue, troop.Position, 0));
        }

        [TestMethod]
        public void Should_ReturnFalse_When_EntersFriend()
        {
            CreateValidator();
            AddTroop(2, 2);
            AddTroop(3, 2);
            DoAddTroops();
            Troop troop = GetTroop(2, 2);

            Assert.IsFalse(validator.IsLegalMove(PlayerSide.Blue, troop.Position, 0));
        }

        [TestMethod]
        public void Should_ReturnTrue_When_BlockedBy3Friends()
        {
            CreateValidator();
            AddTroop(2, 2);

            AddTroop(2, 3);
            AddTroop(3, 2);
            AddTroop(2, 1);
            DoAddTroops();

            Troop troop = GetTroop(2, 2);

            Assert.IsTrue(validator.IsLegalMove(PlayerSide.Blue, troop.Position, 0));
        }
    }
}
