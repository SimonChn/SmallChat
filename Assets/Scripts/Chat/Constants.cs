namespace SmallChat
{
    internal static class Constants
    {
        public const string SaveFileName = "UserData.save";

        internal static class Client
        {
            public const string UserIdFieldName = "userId";

            public const string NickameFieldName = "nickname";
            public const string ColorFieldName = "color";

            public const string IsOnlineFieldName = "isOnline";

            public const string MessageFieldName = "message";
            public const string DefaultUserId = "-1";
        }

        internal static class Server
        {
            public const string MessagesHistory = "messagesHistory";

            internal static class Commands
            {
                public const char CommandResultChar = '/';

                public const string GetSimpleUsersLog = "simpleuserslog";
                public const string GetUserJsonedMessage = "userJsonedMessage";
            }
        }
    }
}
