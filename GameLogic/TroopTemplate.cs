namespace GameServer.GameLogic
{
    public class TroopTemplate
    {
        public PlayerId controllingPlayer;
        public int movePoints;
        public int health;
        public int orientation;

        //TODO: add unit type to be used on frontend

        public TroopTemplate(PlayerId controllingPlayer, int movePoints, int health, int orientation)
        {
            this.controllingPlayer = controllingPlayer;
            this.movePoints = movePoints;
            this.health = health;
            this.orientation = orientation;
        }
    }
}