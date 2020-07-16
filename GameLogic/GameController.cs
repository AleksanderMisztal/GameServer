using System.Linq;
using System.Collections.Generic;
using GameServer.Utils;
using GameServer.Networking;
using System.Threading.Tasks;

namespace GameServer.GameLogic
{
    public class GameController
    {
        public readonly int gameId;
        public bool gameEnded = false;

        private PlayerId activePlayer = PlayerId.Red;
        private int roundNumber = 0;
        private readonly int maxBlueWave = 0;
        private readonly int maxRedWave = 0;

        private int blueScore = 0;
        private int redScore = 0;

        private int movePointsLeft;
        private readonly Dictionary<int, List<TroopTemplate>> waves;
        public BoardParams Board { get; private set; }

        private readonly HashSet<Troop> blueTroops = new HashSet<Troop>();
        private readonly HashSet<Troop> redTroops = new HashSet<Troop>();
        private readonly Dictionary<Vector2Int, Troop> troopAtPosition = new Dictionary<Vector2Int, Troop>();
        private readonly HashSet<Troop> aiControlled = new HashSet<Troop>();

        private PlayerId Oponent => activePlayer == PlayerId.Red ? PlayerId.Blue : PlayerId.Red;


        // Public interface
        public GameController(int gameId)
        {
            this.gameId = gameId;
            waves = TroopSpawns.BasicPlanes(out maxBlueWave, out maxRedWave);
            Board = BoardParams.Standard;
        }

        public async Task Initialize()
        {
            await ToggleActivePlayer();
        }

        public async Task MoveTroop(PlayerId player, Vector2Int position, int direction)
        {
            if (!IsValidMove(player, position, direction, out string message))
            {
                throw new IllegalMoveException(message);
            }

            Troop troop = troopAtPosition[position];
            List<BattleResult> battleResults = new List<BattleResult>();

            troop.MoveInDirection(direction);
            movePointsLeft--;

            if (!troopAtPosition.TryGetValue(troop.Position, out Troop encounter))
            {
                await GameHandler.TroopMoved(gameId, position, direction, battleResults);
                await AdjustTroopPosition(troop);
                if (movePointsLeft <= 0)
                {
                    await ToggleActivePlayer();
                }
                return;
            }

            BattleResult result = new BattleResult(true, true);
            if (encounter.ControllingPlayer != troop.ControllingPlayer)
                result = Battles.GetFightResult(troop, encounter);

            battleResults.Add(result);
            if (result.AttackerDamaged) ApplyDamage(troop);
            if (result.DefenderDamaged) ApplyDamage(encounter);

            troop.JumpForward();
            
            while (troopAtPosition.TryGetValue(troop.Position, out encounter) && troop.Health > 0)
            {
                result = Battles.GetCollisionResult();
                battleResults.Add(result);
                if (result.AttackerDamaged) ApplyDamage(troop);
                if (result.DefenderDamaged) ApplyDamage(encounter);

                troop.JumpForward();
            }

            await GameHandler.TroopMoved(gameId, position, direction, battleResults);

            if (troop.Health > 0) 
            { 
                await AdjustTroopPosition(troop);
            }


            if (gameEnded)
            {
                await GameHandler.GameEnded(gameId, redScore, blueScore);
            }
            else if (movePointsLeft <= 0)
            {
                await ToggleActivePlayer();
            }
        }


        // Private functions
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
                    if (!targetPosition.IsOutside(Board) 
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

        private void ApplyDamage(Troop troop)
        {
            PlayerId oponent = troop.ControllingPlayer == PlayerId.Blue ? PlayerId.Red : PlayerId.Blue;
            IncrementScore(oponent);

            if (troop.ControllingPlayer == activePlayer && troop.MovePoints > 0)
            {
                movePointsLeft--;
            }
            troop.ApplyDamage();
            if (troop.Health <= 0)
            {
                DestroyTroop(troop);
            }
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
            {
                movePointsLeft -= troop.MovePoints;
            }
            if (friendlyTroops.Count == 0 && roundNumber >= maxWave)
            {
                gameEnded = true;
            }
        }

        private async Task AdjustTroopPosition(Troop troop)
        {
            troopAtPosition.Remove(troop.StartingPosition);
            troopAtPosition.Add(troop.Position, troop);

            troop.StartingPosition = troop.Position;

            if (troop.Position.IsOutside(Board))
            {
                aiControlled.Add(troop);
                await ControllWithAI(troop);
            }
        }

        private async Task ControllWithAI(Troop troop)
        {
            Vector2Int target = new Vector2Int((Board.xMax + Board.xMin) / 2, (Board.yMax + Board.yMin) / 2);

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
                await MoveTroop(activePlayer, troop.Position, minDir);

                if (!troop.Position.IsOutside(Board))
                {
                    return;
                }
            }
        }

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

        private async Task ToggleActivePlayer()
        {
            roundNumber++;
            await SpawnNextWave();

            HashSet<Troop> beginningTroops = activePlayer == PlayerId.Blue ? redTroops : blueTroops;
            HashSet<Troop> endingTroops = activePlayer == PlayerId.Blue ? blueTroops : redTroops;

            foreach(var troop in beginningTroops)
            {
                troop.OnTurnBegin();
            }
            foreach (var troop in endingTroops)
            {
                troop.OnTurnEnd();
            }

            activePlayer = Oponent;
            SetInitialMovePointsLeft(activePlayer);

            aiControlled.RemoveWhere(t => !t.Position.IsOutside(Board));
            foreach (var troop in aiControlled)
            {
                if (troop.ControllingPlayer == activePlayer)
                {
                    await ControllWithAI(troop);
                }
            }

            if (movePointsLeft == 0)
            {
                await ToggleActivePlayer();
            }
        }

        private async Task SpawnNextWave()
        {
            if (!waves.TryGetValue(roundNumber, out List<TroopTemplate> wave))
            {
                await GameHandler.TroopsSpawned(gameId, new List<TroopTemplate>());
                return;
            }

            foreach (var template in wave)
            {
                template.position = GetEmptyCell(template.position);
                Troop troop = new Troop(template);

                troopAtPosition.Add(troop.Position, troop);

                if (troop.ControllingPlayer == PlayerId.Blue)
                {
                    blueTroops.Add(troop);
                }
                else
                {
                    redTroops.Add(troop);
                }
            }
            await GameHandler.TroopsSpawned(gameId, wave);
        }

        private void SetInitialMovePointsLeft(PlayerId player)
        {
            HashSet<Troop> troops = player == PlayerId.Blue ? blueTroops : redTroops;
            movePointsLeft = troops.Aggregate(0, (acc, t) => acc + t.InitialMovePoints);
        }
    }
}
