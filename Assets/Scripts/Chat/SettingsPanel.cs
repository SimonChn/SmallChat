using System;
using System.Collections;
using System.Collections.Generic;

using System.Globalization;

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
        private string color = ColorUtility.ToHtmlStringRGB(Color.white);
        
        public void Init(Action<ChatSettings> onSetupEnd)
        {
            applyButton.onClick.AddListener(() => onSetupEnd(new ChatSettings(nickname, color)));
            applyButton.onClick.AddListener(() => Destroy(this.gameObject));

            nicknameField.onEndEdit.AddListener(UpdateNickname);
            colorField.onEndEdit.AddListener(delegate { UpdateColor(colorField); });
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
        }

        private bool IsValidColor(string color)
        {
            if(color.Length > 6)
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


