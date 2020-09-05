using System.Linq;

namespace GameServer.Utils
{
    public static class Serializer
    {
        public static byte[] Deserialize(string data) =>
            data.Split(',').Select(byte.Parse).ToArray();

        public static string Serialize(byte[] bytes) =>
            string.Join(',', bytes);
    }
}
