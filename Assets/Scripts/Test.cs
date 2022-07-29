using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmallChat;

public class Test : MonoBehaviour
{
    [SerializeField] private MessagesViewController messagesViewController;

    private IEnumerator Start()
    {
        messagesViewController.Init();

        var wait = new WaitForSeconds(0.5f);

        for(int i = 1; i < 30; i++)
        {
            yield return wait;

            string s = i.ToString();
            messagesViewController.AddMessage(s);
        }
    }
}
