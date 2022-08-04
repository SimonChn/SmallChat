namespace SmallChatServer
{
    class Program
    {
        private static ServerObject? server;
        private static Thread? listenThread;  

        static void Main()
        {
            try
            {
                server = new ServerObject();
                listenThread = new Thread(new ThreadStart(server.Listen));
                listenThread.Start();
            }
            catch (Exception ex)
            {
                server?.Disconnect();
                Console.WriteLine(ex.Message);
            }
        }
    }
}