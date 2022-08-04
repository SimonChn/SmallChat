using UnityEngine;

using UnityEngine.UI;
using TMPro;

using SmallChat.Client;
using Newtonsoft.Json.Linq;
using System.Text;

namespace SmallChat
{
    public class UsersList : MonoBehaviour, IServerCommandResultListener
    {
        [SerializeField] private Button showUsersListButton;

        [SerializeField] private GameObject usersListPanel;
        [SerializeField] private TextMeshProUGUI usersText;

        private ChatClientController chatClientController;

        public void Init(ChatClientController clientController)
        {
            chatClientController = clientController;
            chatClientController.AddCommandResultListener(Constants.Server.Commands.GetSimpleUsersLog, this);

            showUsersListButton.interactable = true;
            showUsersListButton.onClick.AddListener(() =>
            {
                showUsersListButton.interactable = false;
                chatClientController.SendMessageToServer($"/{Constants.Server.Commands.GetSimpleUsersLog}");
            });
        }

        public void Notify(object data)
        {
            var jArray = data as JArray;

            StringBuilder builder = new StringBuilder();

            string onlineOn = "<sprite=14>";
            string onlineOff = "<sprite=15>";

            foreach (JObject jObject in jArray)
            {
                builder.Append($"{((bool)jObject[Constants.Client.IsOnlineFieldName] ? onlineOn : onlineOff)}   ");
                builder.Append($"<color=#{jObject[Constants.Client.ColorFieldName]}>{jObject[Constants.Client.NickameFieldName]}<color=white>\n");
            }

            showUsersListButton.interactable = true;
            usersText.text = builder.ToString();

            usersListPanel.gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            showUsersListButton.onClick.RemoveAllListeners();
            chatClientController?.RemoveCommandResultListener(Constants.Server.Commands.GetSimpleUsersLog, this);
        }
    }
}
