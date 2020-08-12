using System.Linq;
using System.Collections.Generic;
using GameServer.Utils;
using GameServer.GameLogic.ServerEvents;
using System;

namespace GameServer.GameLogic
{
    public class GameController
    {
        private bool gameEnded = false;

        private PlayerId activePlayer = PlayerId.Red;
        private int roundNumber = 0;
        private readonly int maxBlueWave = 0;
        private readonly int maxRedWave = 0;

        private int blueScore = 0;
        private int redScore = 0;

        private int movePointsLeft;

        private readonly IBattles battles;
        private readonly Waves waves;
        private readonly Board board;

        private readonly HashSet<Troop> blueTroops = new HashSet<Troop>();
        private readonly HashSet<Troop> redTroops = new HashSet<Troop>();
        private readonly Dictionary<Vector2Int, Troop> troopAtPosition = new Dictionary<Vector2Int, Troop>();
        private readonly HashSet<Troop> aiControlled = new HashSet<Troop>();


        public GameController(Waves waves, Board board)
        {
            this.battles = new StandardBattles();
            this.waves = waves;
            this.board = board;
        }

        public GameController(IBattles battles, Board board, Waves waves)
        {
            this.battles = battles;
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

            HashSet<Troop> beginningTroops = activePlayer == PlayerId.Blue ? redTroops : blueTroops;
            HashSet<Troop> endingTroops = activePlayer == PlayerId.Blue ? blueTroops : redTroops;

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
            List<TroopTemplate> wave;
            try
            {
                wave = waves.troopsForRound[roundNumber];

                foreach (var template in wave)
                {
                    template.position = GetEmptyCell(template.position);
                    Troop troop = new Troop(template);

                    troopAtPosition.Add(troop.Position, troop);

                    if (troop.ControllingPlayer == PlayerId.Blue)
                        blueTroops.Add(troop);
                    else
                        redTroops.Add(troop);
                }
            }
            catch (KeyNotFoundException)
            {
                wave = new List<TroopTemplate>();
            }

            return new TroopsSpawnedEvent(wave);
        }

        // TODO: Change from dfs to iterative bfs
        // TODO: Don't return cells outside the board
        private Vector2Int GetEmptyCell(Vector2Int seedPosition)
        {
            if (!troopAtPosition.TryGetValue(seedPosition, out _)) return seedPosition;

            Vector2Int[] neighbours = Hex.GetNeighbours(seedPosition);
            Randomizer.Randomize(neighbours);

            foreach (var position in neighbours)
            {
                if (!troopAtPosition.TryGetValue(position, out _))
                {
                    return position;
                }
            }
            return GetEmptyCell(neighbours[0]);
        }

        private void SetInitialMovePointsLeft(PlayerId player)
        {
            HashSet<Troop> troops = player == PlayerId.Blue ? blueTroops : redTroops;
            movePointsLeft = troops.Aggregate(0, (acc, t) => acc + t.InitialMovePoints);
        }


        public List<IServerEvent> ProcessMoveRequest(PlayerId player, Vector2Int position, int direction)
        {
            List<IServerEvent> events = new List<IServerEvent>();

            if (IsValidMove(player, position, direction, out string message))
            {
                TroopMovedEvent mainMove = MoveTroop(position, direction);
                events.Add(mainMove);

                if (gameEnded)
                {
                    GameEndedEvent gameEndEvent = new GameEndedEvent(redScore, blueScore);
                    events.Add(gameEndEvent);
                }
                else while (movePointsLeft <= 0)
                {
                    var turnEndEvents = ToggleActivePlayerAndReturnEvents();
                    events.AddRange(turnEndEvents);
                }
            }
            else
            {
                // Handle an illegal move
                // In future may send an illegal move event 
                Console.WriteLine($"Illegal move: {message}");
            }

            return events;
        }

        private bool IsValidMove(PlayerId player, Vector2Int position, int direction, out string message)
        {
            message = "";

            if (activePlayer != player)
            {
                message = "Attempting to make a move in oponent's turn!";
                return false;
            }

            if (!troopAtPosition.TryGetValue(position, out Troop troop))
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
            if (troopAtPosition.TryGetValue(targetPosition, out Troop encounter) && encounter.ControllingPlayer == player)
            {
                foreach (var cell in Hex.GetControllZone(troop.Position, troop.Orientation))
                {
                    if (!targetPosition.IsOutside(board) 
                        && (!troopAtPosition.TryGetValue(targetPosition, out encounter) 
                            || encounter.ControllingPlayer != player))
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
            Troop troop = troopAtPosition[position];
            List<BattleResult> battleResults = new List<BattleResult>();

            troop.MoveInDirection(direction);
            movePointsLeft--;

            if (!troopAtPosition.TryGetValue(troop.Position, out Troop encounter))
            {
                AdjustTroopPosition(troop);
                return new TroopMovedEvent(position, direction, battleResults);
            }

            BattleResult result = new BattleResult(true, true);
            if (encounter.ControllingPlayer != troop.ControllingPlayer)
                result = battles.GetFightResult(troop, encounter);

            battleResults.Add(result);
            if (result.AttackerDamaged) ApplyDamage(troop);
            if (result.DefenderDamaged) ApplyDamage(encounter);

            troop.JumpForward();
            
            while (troopAtPosition.TryGetValue(troop.Position, out encounter) && troop.Health > 0)
            {
                result = battles.GetCollisionResult();
                battleResults.Add(result);
                if (result.AttackerDamaged) ApplyDamage(troop);
                if (result.DefenderDamaged) ApplyDamage(encounter);

                troop.JumpForward();
            }

            if (troop.Health > 0) 
            { 
                AdjustTroopPosition(troop);
            }

            return new TroopMovedEvent(position, direction, battleResults);
        }

        private void AdjustTroopPosition(Troop troop)
        {
            troopAtPosition.Remove(troop.StartingPosition);
            troopAtPosition.Add(troop.Position, troop);

            troop.StartingPosition = troop.Position;
        }

        private void ApplyDamage(Troop troop)
        {
            PlayerId opponent = troop.ControllingPlayer.Opponent();
            IncrementScore(opponent);

            if (troop.ControllingPlayer == activePlayer && troop.MovePoints > 0)
                movePointsLeft--;

            troop.ApplyDamage();

            if (troop.Health <= 0)
                DestroyTroop(troop);
        }

        private void IncrementScore(PlayerId player)
        {
            if (player == PlayerId.Blue) blueScore++;
            if (player == PlayerId.Red) redScore++;
        }

        private void DestroyTroop(Troop troop)
        {
            HashSet<Troop> friendlyTroops = troop.ControllingPlayer == PlayerId.Blue ? blueTroops : redTroops;
            int maxWave = troop.ControllingPlayer == PlayerId.Blue ? maxBlueWave : maxRedWave;

            troopAtPosition.Remove(troop.StartingPosition);
            friendlyTroops.Remove(troop);

            if (troop.ControllingPlayer == activePlayer)
                movePointsLeft -= troop.MovePoints;

            if (friendlyTroops.Count == 0 && roundNumber >= maxWave)
                gameEnded = true;
        }

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
                    if (troopAtPosition.TryGetValue(neigh, out _)) continue;

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
