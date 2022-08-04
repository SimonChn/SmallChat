using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmallChat.Client;

namespace SmallChat
{
    public class MessagesViewController : MonoBehaviour, IServerCommandResultListener
    {
        [Range(1, 40)]
        [SerializeField] private int maxMessages = 20;

        [SerializeField] private Transform messagesParent;
        [SerializeField] private MessageView messagePrefab;

        Queue<MessageView> viewSimplePool;

        public void Init()
        {
            viewSimplePool = new Queue<MessageView>();

            for(int i = 0; i < maxMessages; i++)
            {
                var messageView = Instantiate(messagePrefab, messagesParent);
                messageView.gameObject.SetActive(false);
                viewSimplePool.Enqueue(messageView);
            }
        }

        public void Notify(object data)
        {
            string message = MessageParser.GetTextMessageFromObject(data);
            AddMessage(message);
        }

        public void AddMessage(string message)
        {
            var messageView = viewSimplePool.Dequeue();

            viewSimplePool.Enqueue(messageView);

            if (!messageView.gameObject.activeSelf)
            {
                messageView.gameObject.SetActive(true);
            }    

            messageView.transform.SetAsFirstSibling();
            messageView.SetDisplayedMessage(message);
        }
    }
}
