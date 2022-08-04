using StackExchange.Redis;

namespace SmallChatServer
{
    internal static class ExtensionsMethods
    {
        public static string[] ToStringArray(this RedisValue[] values)
        {
            if (values == null || values.Length == 0)
            {
                return Array.Empty<string>();
            }

            var result = new string[values.Length];

            for (int i = 0; i < values.Length; i++)
            {
                result[i] = values[i].ToString();
            }

            return result;
        }
    }
}