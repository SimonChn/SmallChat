using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SmallChat
{

    public class ChatSceneLauncher : MonoBehaviour
    {
        [SerializeField] private Transform settingsPanelParent;
        [SerializeField] private SettingsPanel settingsPanelPrefab;

        private async void Start()
        {
            //Load settings
            var settingsPanel = Instantiate(settingsPanelPrefab, settingsPanelParent);
            settingsPanel.Init((ChatSettings x) => { });
        }
    }
}