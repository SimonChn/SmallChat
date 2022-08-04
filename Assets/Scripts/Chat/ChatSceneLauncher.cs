using System.Collections;
using System.Collections.Generic;

using System.Net.Sockets;
using System.Text;

using UnityEngine;
using SmallChat.Client;

namespace SmallChat
{
    public class ChatSceneLauncher : MonoBehaviour
    {   
        [SerializeField] private Transform settingsPanelParent;
        [SerializeField] private SettingsPanel settingsPanelPrefab;

        [Space(20)]
        [SerializeField] private ChatClientController chatClientController;
        [SerializeField] private MessagesInputController messagesInputController;

        [Space(20)]
        [SerializeField] private UsersList usersList;

        [Header("Messages view")]
        [SerializeField] private MessagesViewController messagesViewController;

        private void Start()
        {
            var settingsPanel = Instantiate(settingsPanelPrefab, settingsPanelParent);
            settingsPanel.Init(OpenChat);
            messagesViewController.Init();
        }

        private void OpenChat(ChatUserSettings settings)
        {
            chatClientController.Init(settings, messagesViewController);

            if(chatClientController.Launch())
            {
                usersList.Init(chatClientController);

                messagesInputController.Init(settings.Color);
                messagesInputController.OnMessageTyped += chatClientController.SendMessageToServer;
                messagesInputController.OnMessageTyped += (message) =>
                {
                    string chatMessage = MessageParser.GetChatTextMessage(settings.Color, settings.Nickname, message);
                    messagesViewController.AddMessage(chatMessage);
                };
            }      
        }
    }
}