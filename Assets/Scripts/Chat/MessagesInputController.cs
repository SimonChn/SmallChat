using System;

using UnityEngine;
using UnityEngine.UI;

using TMPro;
using UnityEngine.EventSystems;

namespace SmallChat
{
    public class MessagesInputController : MonoBehaviour
    {
        [SerializeField] private TMP_InputField messageInputField;
        [SerializeField] private Button sendButton;

        public event Action<string> OnMessageTyped;

        public void Init(string color)
        {
            if (ColorUtility.TryParseHtmlString($"#{color}", out var textColor))
            {
                messageInputField.textComponent.color = textColor;
            }

            sendButton.onClick.AddListener(GetAndSendMessage);
            messageInputField.onValueChanged.AddListener(delegate { CheckTypeStatus(); });
        }

        private void CheckTypeStatus()
        {
            if(string.IsNullOrEmpty(messageInputField.text))
            {
                if(sendButton.gameObject.activeSelf)
                {
                    sendButton.gameObject.SetActive(false);
                }            
            }
            else
            {
                if (!sendButton.gameObject.activeSelf)
                {
                    sendButton.gameObject.SetActive(true);
                }
            }
        }

        private void GetAndSendMessage()
        {
            OnMessageTyped?.Invoke(messageInputField.text);
            messageInputField.text = string.Empty;

            sendButton.gameObject.SetActive(false);
            EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);
        }

        private void OnDisable()
        {
            sendButton.onClick.RemoveListener(GetAndSendMessage);
            messageInputField.onValueChanged.RemoveAllListeners();

            OnMessageTyped = null;
        }
    }
}
