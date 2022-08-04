using Newtonsoft.Json.Linq;

namespace SmallChat
{
    public static class MessageParser
    {
        public static string GetTextMessageFromObject(object messageObject)
        {
            JObject jObject = messageObject as JObject;

            return GetChatTextMessage(jObject[Constants.Client.ColorFieldName].ToString(), 
                jObject[Constants.Client.NickameFieldName].ToString(), jObject[Constants.Client.MessageFieldName].ToString());
        }

        public static string GetChatTextMessage(string color, string nickname, string message)
        {
            return $"<color=#{color}>{nickname}: {message}";
        }
    }
}