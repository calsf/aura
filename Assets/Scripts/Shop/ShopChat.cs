using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ShopChat : MonoBehaviour
{
    [SerializeField]
    Text chatDisplay;

    [SerializeField]
    [TextArea(3, 10)]
    string defaultChat;

    int currChar;
    StringBuilder sb;
    string currChat;


    // Start is called before the first frame update
    void Start()
    {
        // Set default text on entering shop
        sb = new StringBuilder();
        SetChat(defaultChat);
    }

    // Show the new chat text one character at a time
    void FixedUpdate()
    {
        if (chatDisplay.text != currChat && currChar < currChat.Length)
        {
            sb.Append(currChat[currChar]);
            chatDisplay.text = sb.ToString();
            currChar += 1;
        }
    }

    // Set chat based on shop action in ShopManager
    public void SetChat(string chat)
    {
        sb.Clear();
        chatDisplay.text = "";
        
        currChat = chat;
        currChar = 0;
    }
}
