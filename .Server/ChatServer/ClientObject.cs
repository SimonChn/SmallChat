using System.Net.Sockets;
using System.Text;

using Newtonsoft.Json.Linq;

namespace SmallChatServer
{
    public class ClientObject
    {
        protected internal string Id { get; private set; } = Constants.Client.DefaultUserId;
        protected internal NetworkStream? Stream { get; private set; }

        private readonly TcpClient client;
        private readonly ServerObject server;

        private ChatClientSettings chatClientSettings;

        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            client = tcpClient;
            server = serverObject;
        }

        protected internal void Process()
        {
            try
            {
                Stream = client.GetStream();
                
                string logInMessage = GetRawMessage();
                ProceedLogIn(logInMessage);
                
                string message;

                while (true)
                {
                    try
                    {
                        message = GetRawMessage();

                        if(string.IsNullOrEmpty(message))
                        {
                            break;
                        }

                        if(message.StartsWith(Constants.Server.Commands.CommandChar))
                        {
                            ServerCommandExecutor.ExecuteCommand(server, this, message);
                        }
                        else
                        {
                            message = GetJsonedMessage(message);                      
                            server.LogClientMessage(message);
                            server.BroadcastMessage(message, this.Id);
                        }
                    }
                    catch(Exception)
                    {                    
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                string message = $"{chatClientSettings.Nickname} has left the chat";
                Console.WriteLine(message);

                server.BroadcastMessage(message, this.Id);

                server.RemoveConnection(this.Id);
                Close();
            }
        }

        protected internal void Close()
        {
            Stream?.Close();
            client?.Close();
        }

        private string GetRawMessage()
        {
            if (Stream is null)
            {
                return string.Empty;
            }

            byte[] data = new byte[64];
            StringBuilder builder = new StringBuilder();

            do
            {
                int bytes = Stream.Read(data, 0, data.Length);

                if(bytes == 0)
                {
                    throw new IOException();
                }

                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (Stream.DataAvailable);

            return builder.ToString();
        }

        private string GetJsonedMessage(string rawMessage)
        {
            Console.WriteLine($"{chatClientSettings.Nickname}: {rawMessage}");

            JObject messageObject = new JObject()
            {
                    {Constants.Client.NickameFieldName, chatClientSettings.Nickname},
                    {Constants.Client.ColorFieldName, chatClientSettings.Color},
                    {Constants.Client.MessageFieldName, rawMessage}
            };

            return messageObject.ToString(Newtonsoft.Json.Formatting.None);
        }

        private void ProceedLogIn(string logInMessage)
        {
            JObject jObject = JObject.Parse(logInMessage);

            string? nickname = jObject[Constants.Client.NickameFieldName]?.ToObject<string>();
            string? color = jObject[Constants.Client.ColorFieldName]?.ToObject<string>();

            if (nickname is null || color is null)
            {
                Close();
                return;
            }

            string? id = jObject[Constants.Client.UserIdFieldName]?.ToObject<string>();

            if (id != null)
            {
                this.Id = id;
                server.Connect(this, this.Id, nickname, color);
                ServerObject.SendDataToClient(server.GetMessagesLog(), this);
            }
            else
            {
                Id = server.ConnectNew(this, nickname, color);
                ServerObject.SendDataToClient(Id, this);
            }

            if(Id == Constants.Client.DefaultUserId)
            {
                Close();
                return;
            }

            chatClientSettings = new ChatClientSettings(nickname, color);

            string clientJoinedMessage = $"{chatClientSettings.Nickname} has joined the chat";
            Console.WriteLine(clientJoinedMessage);
            server.BroadcastMessage(clientJoinedMessage, this.Id);
        }
    }
}