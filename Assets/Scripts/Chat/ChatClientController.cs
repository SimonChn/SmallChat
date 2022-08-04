using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

using System.Threading;

using Newtonsoft.Json.Linq;

using UnityEngine;
using System.Collections.Concurrent;
using System.Collections;

namespace SmallChat.Client
{
    public class ChatClientController : MonoBehaviour
    {
        private const string host = "127.0.0.1";
        private const int port = 8888;

        private Dictionary<string, List<IServerCommandResultListener>> commandResultsListeners
            = new Dictionary<string, List<IServerCommandResultListener>>();

        private ConcurrentQueue<string> receiveQueue = new ConcurrentQueue<string>();

        private ChatUserSettings userSettings;

        private IServerCommandResultListener textMessagesListener;

        private TcpClient client;
        private NetworkStream stream;

        private Thread receiveThread;

        private bool isLaunched = false;
        private bool isClosed = false;

        public void Init(ChatUserSettings userSettings, IServerCommandResultListener jsonedMessagesListener)
        {
            this.userSettings = userSettings;
            this.textMessagesListener = jsonedMessagesListener;
        }

        public bool Launch()
        {
            if(isLaunched)
            {
                return true;
            }

            isLaunched = true;

            client = new TcpClient();

            try
            {
                client.Connect(host, port);
                stream = client.GetStream();

                JObject logInObject = new JObject()
                {
                    {Constants.Client.NickameFieldName, userSettings.Nickname},
                    {Constants.Client.ColorFieldName, userSettings.Color},
                };

                if (this.userSettings.Id != Constants.Client.DefaultUserId)
                {
                    logInObject.Add(Constants.Client.UserIdFieldName, userSettings.Id);
                }

                string logInMessage = logInObject.ToString();

                byte[] data = Encoding.Unicode.GetBytes(logInMessage);
                stream.Write(data, 0, data.Length);

                receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                receiveThread.Start();

                StartCoroutine(ReadMessagesCoroutine());
            }
            catch (Exception)
            {
                Disconnect();
            }

            return true;
        }

        public void Close()
        {
            if (!isClosed)
            {
                Disconnect();
            }         
        }

        public void AddCommandResultListener(string commandName, IServerCommandResultListener listener)
        {
            if (commandResultsListeners.ContainsKey(commandName))
            {
                commandResultsListeners[commandName].Add(listener);
            }
            else
            {
                commandResultsListeners.Add(commandName, new List<IServerCommandResultListener> { listener });
            }
        }

        public void RemoveCommandResultListener(string commandName, IServerCommandResultListener listener)
        {
            if (commandResultsListeners.ContainsKey(commandName))
            {
                commandResultsListeners[commandName].Remove(listener);
            }
        }

        public void SendMessageToServer(string message)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            stream?.Write(data, 0, data.Length);
        }

        private IEnumerator ReadMessagesCoroutine()
        {
            yield return null;

            while(!isClosed)
            {
                if(receiveQueue.TryDequeue(out string message))                 
                {
                    ProceedLogIn(message);
                    break;
                }

                yield return null;
            }

            while (!isClosed)
            {
                if (receiveQueue.TryDequeue(out var message))
                {
                    if (message.StartsWith(Constants.Server.Commands.CommandResultChar))
                    {
                        ProceedCommandResult(message);
                    }
                    else
                    {
                        try
                        {
                            textMessagesListener?.Notify(JObject.Parse(message));
                        }
                        catch(Exception)
                        {
                            
                        }                      
                    }                
                }

                yield return null;
            }
        }

        private void ReceiveMessage()
        {
            if (stream is null)
            {
                return;
            }

            try
            {
                while (true)
                {
                    try
                    {
                        string message = GetRawMessage();
                        receiveQueue.Enqueue(message);
                    }
                    catch (Exception)
                    {
                        break;
                    }
                }
            }
            catch (Exception)
            {
                Disconnect();
            }
        }

        private void ProceedCommandResult(string commandResult)
        {
            commandResult = commandResult[1..];
            JObject command = JObject.Parse(commandResult);

            foreach (JProperty property in command.Properties())
            {
                if (commandResultsListeners.ContainsKey(property.Name))
                {
                    var listeners = commandResultsListeners[property.Name];
                    foreach (var listener in listeners)
                    {                    
                        listener.Notify(property.Value);
                    }
                }
            }
        }

        private string GetRawMessage()
        {
            if (stream is null)
            {
                return string.Empty;
            }

            byte[] data = new byte[64];
            StringBuilder builder = new StringBuilder();

            do
            {
                int bytes = stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (stream.DataAvailable);

            return builder.ToString();
        }

        private void ProceedLogIn(string logInMessage)
        {
            if (this.userSettings.Id == Constants.Client.DefaultUserId)
            {
                this.userSettings = new ChatUserSettings(userSettings.Nickname, userSettings.Color, logInMessage);
                userSettings.Save();
            }
            else
            {
                if(textMessagesListener == null)
                {
                    return;
                }

                var historyObject = JObject.Parse(logInMessage);

                if(historyObject == null)
                {
                    return;
                }

                var jObjectMessages = historyObject[Constants.Server.MessagesHistory].ToObject<JArray>();

                foreach (var jObjectMessage in jObjectMessages)
                {
                    textMessagesListener.Notify(jObjectMessage);
                }
            }
        }

        private void OnDisable()
        {
            Close();
        }

        private void Disconnect()
        {
            StopAllCoroutines();

            stream?.Close();
            client?.Close();

            isClosed = true;

            textMessagesListener = null;
            commandResultsListeners.Clear();
            receiveQueue.Clear();
        }
    }
}