using System;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace SmallChat
{
    public class SettingsPanel : MonoBehaviour
    {
        [SerializeField] private TMP_InputField nicknameField;
        [SerializeField] private TMP_InputField colorField;

        [SerializeField] private Button applyButton;

        private string nickname = string.Empty;
        private string color = "FFFFFF";
        
        public void Init(Action<ChatUserSettings> onSetupEnd)
        {
            ChatUserSettings loadedSettings = ChatUserSettings.TryLoad(Application.persistentDataPath);

            if(loadedSettings != null)
            {
                nickname = loadedSettings.Nickname;
                color = loadedSettings.Color;

                nicknameField.text = nickname;
                colorField.text = color;
                if (ColorUtility.TryParseHtmlString($"#{color}", out var textColor))
                {
                    colorField.textComponent.color = textColor;
                }

                applyButton.onClick.AddListener(() =>
                {
                    if(nickname != loadedSettings.Nickname || color != loadedSettings.Color)
                    {
                        var newSettings = new ChatUserSettings(nickname, color, loadedSettings.Id);
                        newSettings.Save();
                        onSetupEnd(newSettings);
                    }
                    else
                    {
                        onSetupEnd(loadedSettings);
                    }              
                });
            }
            else
            {
                applyButton.onClick.AddListener(() => onSetupEnd(new ChatUserSettings(nickname, color)));          
            }

            applyButton.onClick.AddListener(() => DestroySelf());

            nicknameField.onEndEdit.AddListener(UpdateNickname);
            colorField.onEndEdit.AddListener(delegate { UpdateColor(colorField); });
        }

        private void DestroySelf()
        {
            applyButton.onClick.RemoveAllListeners();
            nicknameField.onEndEdit.RemoveAllListeners();
            colorField.onEndEdit.RemoveAllListeners();

            Destroy(this.gameObject);
        }

        private void UpdateNickname(string newNickname)
        {
            nickname = newNickname;
        }

        private void UpdateColor(TMP_InputField colorField)
        {
            if(!IsValidColor(colorField.text))
            {
                colorField.text = color;
            }
            else
            {
                color = colorField.text;
            }

            if(ColorUtility.TryParseHtmlString($"#{colorField.text}", out var textColor))
            {
                colorField.textComponent.color = textColor;
            }

        }
        private bool IsValidColor(string color)
        {
            if(color.Length != 6)
            {
                return false;
            }

            color = color.ToLower();

            //39 is 'A' - '0', 15 is max hex value

            for(int i = 0; i < color.Length; i++)
            {
                int value = (color[i] - '0') % 39;

                if (value > 15)
                {
                    return false;
                }
            }

            return true;
        }
    }
}


