using System;
using System.Linq;
using System.Collections.Generic;
using GameServer.GameLogic.Battles;
using GameServer.GameLogic.GameEvents;
using GameServer.GameLogic.Utils;

namespace GameServer.GameLogic
{
    public class GameController
    {
        private PlayerSide activePlayer = PlayerSide.Red;
        private int roundNumber;
        private int movePointsLeft;

        private readonly Score score = new Score();

        private readonly IBattleResolver battleResolver;
        private readonly Waves.Waves waves;
        private readonly Board board;
        private readonly TroopMap troopMap;
        private readonly MoveValidator validator;
        private readonly TroopAi troopAi;


        public GameController(Waves.Waves waves, Board board)
        {
            battleResolver = new StandardBattles();
            this.waves = waves;
            this.board = board;
            troopMap = new TroopMap(board);
            validator = new MoveValidator(troopMap, board, activePlayer);
            troopAi = new TroopAi(troopMap, board);
        }

        public GameController(IBattleResolver battleResolver, Board board, Waves.Waves waves)
        {
            this.battleResolver = battleResolver;
            this.waves = waves;
            this.board = board;
            troopMap = new TroopMap(board);
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

        private List<IGameEvent> ToggleActivePlayerAndReturnEvents()
        {
            roundNumber++;
            List<IGameEvent> events = new List<IGameEvent>();

            NewRoundEvent troopsSpawnedEvent = AddSpawnsForCurrentRoundAndReturnEvent();
            events.Add(troopsSpawnedEvent);

            ChangeActivePlayer();

            List<TroopMovedEvent> aiMoveEvents = ExecuteAiMoves();
            events.AddRange(aiMoveEvents);

            return events;
        }

        private void ChangeActivePlayer()
        {
            HashSet<Troop> beginningTroops = troopMap.GetTroops(activePlayer.Opponent());
            foreach (Troop troop in beginningTroops)
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


        public List<IGameEvent> ProcessMove(PlayerSide player, Vector2Int position, int direction)
        {
            List<IGameEvent> events = new List<IGameEvent>();

            if (!validator.IsLegalMove(player, position, direction)) return events;
            Troop troop = troopMap.Get(position);
            TroopMovedEvent mainMove = MoveTroop(position, direction);
            events.Add(mainMove);
            if (board.IsOutside(troop.Position))
            {
                List<TroopMovedEvent> aiMoveEvents = ControlWithAi(troop);
                events.AddRange(aiMoveEvents);
            }

            while (!GameHasEnded())
            {
                if (movePointsLeft == 0)
                {
                    List<IGameEvent> turnEndEvents = ToggleActivePlayerAndReturnEvents();
                    events.AddRange(turnEndEvents);
                }
                else return events;
            }
            GameEndedEvent gameEndEvent = new GameEndedEvent(score);
            events.Add(gameEndEvent);

            return events;
        }

        private List<TroopMovedEvent> ControlWithAi(Troop troop)
        {
            List<TroopMovedEvent> events = new List<TroopMovedEvent>();
            while (troopAi.ShouldControl(troop) && troop.MovePoints > 0)
            {
                int direction = troopAi.GetOptimalDirection(troop);
                TroopMovedEvent moveEvent = MoveTroop(troop.Position, direction);
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
            movePointsLeft--;

            Troop troop = troopMap.Get(position);
            troop.MoveInDirection(direction);

            List<BattleResult> battleResults = new List<BattleResult>();
            Troop encounter = troopMap.Get(troop.Position);
            if (encounter == null)
            {
                troopMap.AdjustPosition(troop);
                return new TroopMovedEvent(position, direction, battleResults);
            }

            BattleResult result = BattleResult.FriendlyCollision;
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
                troopMap.AdjustPosition(troop);

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
            foreach (Troop troop in troopMap.GetTroops(activePlayer))
            {
                if (!troopAi.ShouldControl(troop)) continue;
                List<TroopMovedEvent> moveEvents = ControlWithAi(troop);
                events.AddRange(moveEvents);
            }
            return events;
        }
    }
}
