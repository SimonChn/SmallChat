using System.Net.Sockets;
using System.Net;
using System.Text;

using StackExchange.Redis;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SmallChatServer
{
    public class ServerObject
    {
        private static ConnectionMultiplexer? databaseConnection;
        private static TcpListener? tcpListener;
        private readonly List<ClientObject> onlineClients = new List<ClientObject>();

        private IDatabase? database = null;

        protected internal static void SendDataToClient(string serializedData, ClientObject clientObject)
        {
            byte[] data = Encoding.Unicode.GetBytes(serializedData);
            clientObject.Stream?.Write(data, 0, data.Length);
        }

        protected internal void Listen()
        {
            try
            {
                InitDatabase();

                tcpListener = new TcpListener(IPAddress.Any, 8888);
                tcpListener.Start();
                Console.WriteLine("Server started");

                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();

                    ClientObject clientObject = new ClientObject(tcpClient, this);
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }

        protected internal void Connect(ClientObject clientObject, string userId, string nickname, string textColor)
        {
            UpdateUserData(userId, nickname, textColor, isOnline: true);
            onlineClients.Add(clientObject);
        }

        protected internal string ConnectNew(ClientObject clientObject, string nickname, string textColor)
        {
            if(database is null)
            {
                return Constants.Client.DefaultUserId;
            }

            long newUserNumber = database.StringIncrement(Constants.Database.TotalUsersCountKey);
            string newUserId = $"{Constants.Database.UserIDKey}:{newUserNumber}";

            UpdateUserData(newUserId, nickname, textColor, isOnline: true);
            database.SetAdd(Constants.Database.UsersSet, newUserId);

            onlineClients.Add(clientObject);
            return newUserId;
        }

        protected internal void RemoveConnection(string id)
        {       
            ClientObject? client = onlineClients.FirstOrDefault(c => c.Id == id);
            
            if (client is not null)
            {
                onlineClients.Remove(client);
                database?.HashSet(id, Constants.Database.UserOnlineStatus, false.ToString());
            }
        }

        protected internal void Disconnect()
        {
            tcpListener?.Stop();

            for (int i = 0; i < onlineClients.Count; i++)
            {
                onlineClients[i].Close();
            }

            databaseConnection?.Close();
            databaseConnection?.Dispose();

            Environment.Exit(0);
        }

        protected internal void BroadcastMessage(string message, string publisherID)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            for (int i = 0; i < onlineClients.Count; i++)
            {
                if (onlineClients[i].Id != publisherID)
                {
                    onlineClients[i].Stream?.Write(data, 0, data.Length);
                }
            }
        }

        protected internal void LogClientMessage(string message)
        {
            if(database is null)
            {
                return;
            }

            string logKey = Constants.Database.MessagesHistory;
            database.ListRightPush(logKey, message);

            if(database.ListLength(logKey) > Constants.Database.MaxMessagesFromHistory)
            {
                database.ListLeftPop(logKey);
            }
        }
        
        protected internal string GetMessagesLog()
        {
            if(database is null)
            {
                return string.Empty;
            }

            var messagesDbLog = database.ListRange(Constants.Database.MessagesHistory,
                - Constants.Database.MasStoredHistoryMessages, Constants.Database.MasStoredHistoryMessages);
            var messagesLogArray = messagesDbLog.ToStringArray();

            JArray messagesArray = new JArray();

            if (messagesLogArray is not null)
            {
                for (int i = 0; i < messagesLogArray.Length; i++)
                {
                    var res = JsonConvert.DeserializeObject<JObject>(messagesLogArray[i]);
                    if (res is not null)
                    {
                        messagesArray.Add(res);
                    }          
                }
            }

            JObject messagesLogObject = new JObject(new JProperty(Constants.Server.MessagesHistory, messagesArray));
            return messagesLogObject.ToString();
        }

        protected internal string GetSimpleUsersLog()
        {          
            JArray usersArray = new JArray();
            JObject res = new JObject(new JProperty(Constants.Server.Commands.GetSimpleUsersLog, usersArray));

            if (database is null)
            {
                return res.ToString();
            }

            var users = database.SetMembers(Constants.Database.UsersSet);

            if (users is not null)
            {
                for (int i = 0; i < users.Length; i++)
                {
                    string userId = users[i].ToString();
                    var values = database.HashGetAll(userId);

                    string? nickname = null;
                    string? color = null;
                    bool? isOnline = null;

                    for (int j = 0; j < values.Length; j++)
                    {
                        if(values[j].Name == Constants.Database.UserNickname)
                        {
                            nickname = values[j].Value;
                        }
                        else if (values[j].Name == Constants.Database.UserTextColor)
                        {
                            color = values[j].Value;
                        }
                        else if (values[j].Name == Constants.Database.UserOnlineStatus)
                        {
                            isOnline = Convert.ToBoolean(values[j].Value);
                        }                      
                    }

                    if(nickname != null && color != null && isOnline != null)
                    {
                        JObject userObject = new JObject()
                        {
                            {Constants.Client.NickameFieldName, nickname},
                            {Constants.Client.ColorFieldName, color},
                            {Constants.Client.IsOnlineFieldName, isOnline}
                        };

                        usersArray.Add(userObject);
                    }
                }
            }

            return res.ToString();
        }

        private void InitDatabase()
        {
            databaseConnection = ConnectionMultiplexer.Connect("localhost");
            database = databaseConnection.GetDatabase();
            Console.WriteLine("Database connected");

            if(!database.KeyExists(Constants.Database.TotalUsersCountKey))
            {
                database.StringSet(Constants.Database.TotalUsersCountKey, 0);
            }
        }

        private void UpdateUserData(string userId, string nickname, string textColor, bool isOnline)
        {
            database?.HashSet(userId, new HashEntry[]
            {
                    new HashEntry(Constants.Database.UserNickname, nickname),
                    new HashEntry(Constants.Database.UserTextColor, textColor),
                    new HashEntry(Constants.Database.UserOnlineStatus, isOnline.ToString())
            });
        }      
    }
}