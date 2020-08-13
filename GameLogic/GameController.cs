using System;
using System.Linq;
using System.Collections.Generic;
using GameServer.Utils;
using GameServer.GameLogic.ServerEvents;

namespace GameServer.GameLogic
{
    public class GameController
    {
        private PlayerSide activePlayer = PlayerSide.Red;
        private int roundNumber = 0;
        private int movePointsLeft;

        private readonly Score score = new Score();

        private readonly IBattleResolver battleResolver;
        private readonly Waves waves;
        private readonly Board board;
        private readonly TroopMap troopMap = new TroopMap();
        private readonly MoveValidator validator;
        private readonly TroopAi troopAi;


        public GameController(Waves waves, Board board)
        {
            battleResolver = new StandardBattles();
            this.waves = waves;
            this.board = board;
            validator = new MoveValidator(troopMap, board, activePlayer);
            troopAi = new TroopAi(troopMap, board);
        }

        public GameController(IBattleResolver battleResolver, Board board, Waves waves)
        {
            this.battleResolver = battleResolver;
            this.waves = waves;
            this.board = board;
            validator = new MoveValidator(troopMap, board, activePlayer);
            troopAi = new TroopAi(troopMap, board);
        }

        
        public NewRoundEvent InitializeAndReturnEvent()
        {
            if (roundNumber == 0)
            {
                return (NewRoundEvent)ToggleActivePlayerAndReturnEvents()[0];
            }
            throw new Exception("This game controller has already been initialized");
        }

        private List<IServerEvent> ToggleActivePlayerAndReturnEvents()
        {
            roundNumber++;
            List<IServerEvent> events = new List<IServerEvent>();

            var troopsSpawnedEvent = AddSpawnsForCurrentRoundAndReturnEvent();
            events.Add(troopsSpawnedEvent);

            ChangeActivePlayer();

            var aiMoveEvents = ExecuteAiMoves();
            events.AddRange(aiMoveEvents);

            return events;
        }

        private void ChangeActivePlayer()
        {
            HashSet<Troop> beginningTroops = troopMap.GetTroops(activePlayer.Opponent());
            foreach (var troop in beginningTroops)
                troop.ResetMovePoints();

            activePlayer = activePlayer.Opponent();
            validator.ToggleActivePlayer();
            SetInitialMovePointsLeft(activePlayer);
        }

        private NewRoundEvent AddSpawnsForCurrentRoundAndReturnEvent()
        {
            List<Troop> wave = waves.GetTroops(roundNumber);
            wave = troopMap.SpawnWave(wave);
            return new NewRoundEvent(wave);
        }

        private void SetInitialMovePointsLeft(PlayerSide player)
        {
            HashSet<Troop> troops = troopMap.GetTroops(player);
            movePointsLeft = troops.Aggregate(0, (acc, t) => acc + t.InitialMovePoints);
        }


        public List<IServerEvent> ProcessMove(PlayerSide player, Vector2Int position, int direction)
        {
            List<IServerEvent> events = new List<IServerEvent>();

            if (validator.IsLegalMove(player, position, direction, board))
            {
                Troop troop = troopMap.Get(position);
                TroopMovedEvent mainMove = MoveTroop(position, direction);
                events.Add(mainMove);
                if (board.IsOutside(troop.Position))
                {
                    var aiMoveEvents = ControllWithAi(troop);
                    events.AddRange(aiMoveEvents);
                }

                while (!GameHasEnded())
                {
                    if (movePointsLeft == 0)
                    {
                        var turnEndEvents = ToggleActivePlayerAndReturnEvents();
                        events.AddRange(turnEndEvents);
                    }
                    else return events;
                }
                GameEndedEvent gameEndEvent = new GameEndedEvent(score);
                events.Add(gameEndEvent);
            }

            return events;
        }

        private List<TroopMovedEvent> ControllWithAi(Troop troop)
        {
            List<TroopMovedEvent> events = new List<TroopMovedEvent>();
            while (troopAi.ShouldControll(troop) && troop.MovePoints > 0)
            {
                int direction = troopAi.GetOptimalDirection(troop);
                var moveEvent = MoveTroop(troop.Position, direction);
                events.Add(moveEvent);
            }
            return events;
        }

        private bool GameHasEnded()
        {
            bool redLost = troopMap.GetTroops(PlayerSide.Red).Count == 0 && waves.maxRedWave <= roundNumber;
            bool blueLost = troopMap.GetTroops(PlayerSide.Blue).Count == 0 && waves.maxBlueWave <= roundNumber;

            return redLost || blueLost;
        }

        private TroopMovedEvent MoveTroop(Vector2Int position, int direction)
        {
            Troop troop = troopMap.Get(position);
            List<BattleResult> battleResults = new List<BattleResult>();

            troop.MoveInDirection(direction);
            movePointsLeft--;

            Troop encounter = troopMap.Get(troop.Position);
            if (encounter == null)
            {
                troopMap.AdjustPosition(troop);
                return new TroopMovedEvent(position, direction, battleResults);
            }

            BattleResult result = new BattleResult(true, true);
            if (encounter.Player != troop.Player)
                result = battleResolver.GetFightResult(troop, encounter);

            battleResults.Add(result);
            if (result.AttackerDamaged) ApplyDamage(troop);
            if (result.DefenderDamaged) ApplyDamage(encounter);

            troop.FlyOverOtherTroop();
            
            while ((encounter = troopMap.Get(troop.Position)) != null && troop.Health > 0)
            {
                result = battleResolver.GetCollisionResult();
                battleResults.Add(result);
                if (result.AttackerDamaged) ApplyDamage(troop);
                if (result.DefenderDamaged) ApplyDamage(encounter);

                troop.FlyOverOtherTroop();
            }

            if (troop.Health > 0)
            { 
                troopMap.AdjustPosition(troop);
            }

            return new TroopMovedEvent(position, direction, battleResults);
        }

        private void ApplyDamage(Troop troop)
        {
            PlayerSide opponent = troop.Player.Opponent();
            score.Increment(opponent);

            if (troop.Player == activePlayer && troop.MovePoints > 0)
                movePointsLeft--;

            troop.ApplyDamage();

            if (troop.Health <= 0)
                DestroyTroop(troop);
        }

        private void DestroyTroop(Troop troop)
        {
            troopMap.Remove(troop);

            if (troop.Player == activePlayer)
                movePointsLeft -= troop.MovePoints;
        }

        private List<TroopMovedEvent> ExecuteAiMoves()
        {
            List<TroopMovedEvent> events = new List<TroopMovedEvent>();
            foreach (var troop in troopMap.GetTroops(activePlayer))
            {
                if (troopAi.ShouldControll(troop))
                {
                    var moveEvents = ControllWithAi(troop);
                    events.AddRange(moveEvents);
                }
            }
            return events;
        }
    }
}
