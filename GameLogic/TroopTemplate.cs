using GameServer.Utils;

namespace GameServer.GameLogic
{
    public class TroopTemplate
    {
        public PlayerId player;
        public int movePoints;
        public int health;
        public int orientation;
        public Vector2Int position = new Vector2Int();

        //TODO: add unit type to be used on frontend

        public TroopTemplate(PlayerId controllingPlayer, int movePoints, int health, int orientation)
        {
            this.player = controllingPlayer;
            this.movePoints = movePoints;
            this.health = health;
            this.orientation = orientation;
        }

        public TroopTemplate Deploy(int x, int y)
        {
            TroopTemplate template = new TroopTemplate(player, movePoints, health, orientation);
            template.position = new Vector2Int(x, y);
            return template;
        }

        public TroopTemplate Deploy(Vector2Int position)
        {
            TroopTemplate template = new TroopTemplate(player, movePoints, health, orientation);
            template.position = position;
            return template;
        }

        public override string ToString()
        {
            return $"cp: {player}, mp: {movePoints}, h: {health}, o: {orientation}, p: {position}";
        }
    }
}