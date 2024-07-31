using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    //da vedere se vogliamo un massimo quindi per ora metto un botto ma non so se è l'ideale 
    // dal punto di vista delle performance
    public int maxMessagesInChat = 10000;

    public GameObject chatPanel, textObject;
    
    [SerializeField]
    private List<Message> messageList = new List<Message>();
    
    void Start()
    {
        
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            SendMessageToChat("chatter parla");
        }
    }

    public void SendMessageToChat(string text)
    {
        if (messageList.Count >= maxMessagesInChat)
        {
            Destroy(messageList[0].textObject.gameObject);
            messageList.Remove(messageList[0]);
        }
        Message newMessage = new Message();
        newMessage.text = text;
        GameObject newText = Instantiate(textObject, chatPanel.transform);
        newMessage.textObject = newText.GetComponent<TMP_Text>();
        newMessage.textObject.text = newMessage.text;
        messageList.Add(newMessage);
    }
}

[System.Serializable]
public class Message
{
    public string text;
    public TMP_Text textObject;
}
