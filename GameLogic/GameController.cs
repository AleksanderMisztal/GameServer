using System;
using System.Linq;
using System.Collections.Generic;
using GameServer.Utils;
using GameServer.GameLogic.ServerEvents;

namespace GameServer.GameLogic
{
    public class GameController
    {
        private PlayerId activePlayer = PlayerId.Red;
        private int roundNumber = 0;
        private int movePointsLeft;

        private readonly Score score;

        private readonly IBattleResolver battleResolver;
        private readonly Waves waves;
        private readonly Board board;

        private readonly TroopMap troopMap = new TroopMap();
        private readonly HashSet<Troop> aiControlled = new HashSet<Troop>();


        public GameController(Waves waves, Board board)
        {
            this.battleResolver = new StandardBattles();
            this.waves = waves;
            this.board = board;
        }

        public GameController(IBattleResolver battles, Board board, Waves waves)
        {
            this.battleResolver = battles;
            this.waves = waves;
            this.board = board;
        }


        public TroopsSpawnedEvent InitializeAndReturnEvents()
        {
            if (roundNumber == 0)
            {
                return AddSpawnsForCurrentRoundAndReturnEvent();
            }
            throw new Exception("This game controller has already been initialized");
        }

        private List<IServerEvent> ToggleActivePlayerAndReturnEvents()
        {
            roundNumber++;

            List<IServerEvent> events = new List<IServerEvent>();

            var troopsSpawnedEvent = AddSpawnsForCurrentRoundAndReturnEvent();
            events.Add(troopsSpawnedEvent);

            HashSet<Troop> beginningTroops = troopMap.GetTroops(activePlayer.Opponent());
            HashSet<Troop> endingTroops = troopMap.GetTroops(activePlayer);

            foreach (var troop in beginningTroops)
                troop.OnTurnBegin();
            foreach (var troop in endingTroops)
                troop.OnTurnEnd();

            activePlayer = activePlayer.Opponent();
            SetInitialMovePointsLeft(activePlayer);

            foreach (var troop in aiControlled)
            {
                if (troop.ControllingPlayer == activePlayer)
                {
                    var moveEvents = ControllWithAI(troop);
                    events.AddRange(moveEvents);
                }
            }

            return events;
        }

        private TroopsSpawnedEvent AddSpawnsForCurrentRoundAndReturnEvent()
        {
            List<TroopTemplate> wave = waves.GetTroops(roundNumber);

            foreach (var template in wave)
            {
                template.position = troopMap.GetEmptyCell(template.position);
                Troop troop = new Troop(template);

                troopMap.Add(troop);
            }

            return new TroopsSpawnedEvent(wave);
        }

        private void SetInitialMovePointsLeft(PlayerId player)
        {
            HashSet<Troop> troops = troopMap.GetTroops(player);
            movePointsLeft = troops.Aggregate(0, (acc, t) => acc + t.InitialMovePoints);
        }


        public List<IServerEvent> ProcessMoveRequest(PlayerId player, Vector2Int position, int direction)
        {
            List<IServerEvent> events = new List<IServerEvent>();

            if (IsValidMove(player, position, direction, out string message))
            {
                TroopMovedEvent mainMove = MoveTroop(position, direction);
                events.Add(mainMove);

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
            else
            {
                // Handle an illegal move
                // In future may send an illegal move event 
                Console.WriteLine($"Illegal move: {message}");
            }

            return events;
        }

        private bool GameHasEnded()
        {
            bool redLost = troopMap.GetTroops(PlayerId.Red).Count == 0 && waves.maxRedWave <= roundNumber;
            bool blueLost = troopMap.GetTroops(PlayerId.Blue).Count == 0 && waves.maxBlueWave <= roundNumber;

            return redLost || blueLost;
        }

        private bool IsValidMove(PlayerId player, Vector2Int position, int direction, out string message)
        {
            message = "";

            if (activePlayer != player)
            {
                message = "Attempting to make a move in oponent's turn!";
                return false;
            }

            Troop troop = troopMap.Get(position);
            if (troop == null)
            {
                message = "No troop at the specified hex!";
                return false;
            }

            if (troop.ControllingPlayer != player)
            {
                message = "Attempting to move enemy troop!";
                return false;
            }

            if (troop.MovePoints <= 0)
            {
                message = "Attempting to move a troop with no move points!";
                return false;
            }

            Vector2Int targetPosition = troop.GetAdjacentHex(direction);
            Troop encounter = troopMap.Get(targetPosition);
            if (encounter != null && encounter.ControllingPlayer == player)
            {
                foreach (var cell in Hex.GetControllZone(troop.Position, troop.Orientation))
                {
                    encounter = troopMap.Get(targetPosition);
                    if (!targetPosition.IsOutside(board)
                        && (encounter != null || encounter.ControllingPlayer != player))
                    {
                        message = "Attempting to enter a cell with friendly troop!";
                        return false;
                    }
                }
            }

            return true;
        }

        private TroopMovedEvent MoveTroop(Vector2Int position, int direction)
        {
            Troop troop = troopMap.Get(position);
            List<BattleResult> battleResults = new List<BattleResult>();

            troop.MoveInDirection(direction);
            movePointsLeft--;

            Troop encounter = troopMap.Get(troop.Position);
            if (encounter != null)
            {
                troopMap.AdjustPosition(troop);
                return new TroopMovedEvent(position, direction, battleResults);
            }

            BattleResult result = new BattleResult(true, true);
            if (encounter.ControllingPlayer != troop.ControllingPlayer)
                result = battleResolver.GetFightResult(troop, encounter);

            battleResults.Add(result);
            if (result.AttackerDamaged) ApplyDamage(troop);
            if (result.DefenderDamaged) ApplyDamage(encounter);

            troop.JumpForward();
            
            while ((encounter = troopMap.Get(troop.Position)) != null && troop.Health > 0)
            {
                result = battleResolver.GetCollisionResult();
                battleResults.Add(result);
                if (result.AttackerDamaged) ApplyDamage(troop);
                if (result.DefenderDamaged) ApplyDamage(encounter);

                troop.JumpForward();
            }

            if (troop.Health > 0)
            { 
                troopMap.AdjustPosition(troop);
            }

            return new TroopMovedEvent(position, direction, battleResults);
        }

        private void ApplyDamage(Troop troop)
        {
            PlayerId opponent = troop.ControllingPlayer.Opponent();
            score.Increment(opponent);

            if (troop.ControllingPlayer == activePlayer && troop.MovePoints > 0)
                movePointsLeft--;

            troop.ApplyDamage();

            if (troop.Health <= 0)
                DestroyTroop(troop);
        }

        private void DestroyTroop(Troop troop)
        {
            troopMap.Remove(troop);

            if (troop.ControllingPlayer == activePlayer)
                movePointsLeft -= troop.MovePoints;
        }

        // TODO: Extract a TroopAI class
        private List<TroopMovedEvent> ControllWithAI(Troop troop)
        {
            Vector2Int target = board.GetCenter();

            List<TroopMovedEvent> events = new List<TroopMovedEvent>();

            while (troop.MovePoints > 0)
            {
                int minDist = 1000000;
                int minDir = 0;
                for (int dir = -1; dir <= 1; dir += 2)
                {
                    Vector2Int neigh = Hex.GetAdjacentHex(troop.Position, (6 + dir + troop.Orientation) % 6);
                    if (troopMap.Get(neigh) != null) continue;

                    int dist = (target - neigh).SqrMagnitude;
                    if (dist < minDist)
                    {
                        minDist = dist;
                        minDir = dir;
                    }
                }

                var troopMovedEvent = MoveTroop(troop.Position, minDir);
                events.Add(troopMovedEvent);

                if (!troop.Position.IsOutside(board))
                {
                    aiControlled.Remove(troop);
                    break;
                }
            }
            return events;
        }
    }
}
