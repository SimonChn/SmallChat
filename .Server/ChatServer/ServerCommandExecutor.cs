namespace SmallChatServer
{
    internal static class ServerCommandExecutor
    {
        public const char CommandSign = '/';
        private const char commandSplitSign = ' ';

        public static void ExecuteCommand(ServerObject server, ClientObject invoker, string command)
        {
            var commandSplit = command.Split(commandSplitSign);

            if(commandSplit.Length == 1)
            {
                ExecuteSimpleCommand(server, invoker, commandSplit[0][1..]);
            }
        }

        private static void ExecuteSimpleCommand(ServerObject server, ClientObject invoker, string commandName)
        {
            switch (commandName)
            {
                case Constants.Server.Commands.GetSimpleUsersLog:
                    {
                        string simpleUsersLogData = $"/{server.GetSimpleUsersLog()}";
                        ServerObject.SendDataToClient(simpleUsersLogData,invoker);
                        return;
                    }
                default: return;
            }
        }
    }
}
