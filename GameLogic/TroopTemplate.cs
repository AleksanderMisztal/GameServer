using GameServer.Utils;

namespace GameServer.GameLogic
{
    public class TroopTemplate
    {
        public PlayerId controllingPlayer;
        public int movePoints;
        public int health;
        public int orientation;
        public Vector2Int position = new Vector2Int();

        //TODO: add unit type to be used on frontend

        public TroopTemplate(PlayerId controllingPlayer, int movePoints, int health, int orientation)
        {
            this.controllingPlayer = controllingPlayer;
            this.movePoints = movePoints;
            this.health = health;
            this.orientation = orientation;
        }

        public TroopTemplate Deploy(int x, int y)
        {
            TroopTemplate template = new TroopTemplate(controllingPlayer, movePoints, health, orientation);
            template.position = new Vector2Int(x, y);
            return template;
        }
    }
}