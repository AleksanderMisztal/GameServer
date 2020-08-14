namespace GameServer.Networking
{
    public class User
    {
        public readonly int id;
        public readonly string name;

        public User(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }
}
