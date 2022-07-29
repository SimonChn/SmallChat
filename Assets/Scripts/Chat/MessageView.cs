using UnityEngine;
using TMPro;

namespace SmallChat
{ 
    public class MessageView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI messageText;

        public void SetDisplayedMessage(string message)
        {
            messageText.text = message;
        }
    }
}