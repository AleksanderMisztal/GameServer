using System.Linq;

namespace WebSocketServerAppTest.Utils
{
    public static class Serializer
    {
        public static byte[] Deserialize(string data) =>
            data.Split(',').Select(s => byte.Parse(s)).ToArray();

        public static string Serialize(byte[] bytes) =>
            string.Join(',', bytes);
    }
}
