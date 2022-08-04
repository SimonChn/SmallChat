namespace SmallChatServer
{
    internal static class Constants
    {
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
                public const char CommandChar = '/';
                public const string GetSimpleUsersLog = "simpleuserslog";
            }
        }

        internal static class Database
        {
            public const string TotalUsersCountKey = "total_users";
            public const string UsersSet = "users";

            public const string UserIDKey = "user_id";
            public const string UserNickname = "nickname";
            public const string UserTextColor = "text_color";
            public const string UserOnlineStatus = "is_online";

            public const string MessagesHistory = "messages_history";
            public const int MaxMessagesFromHistory = 20;
            public const int MasStoredHistoryMessages = 1000;
        }
    }
}
