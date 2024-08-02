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
    public float timerChat = 3f;

    public GameObject chatPanel, textObject;
    public ScrollRect scrollRect;
    
    [SerializeField]
    private List<Message> messageList = new List<Message>();
    
    void Start()
    {
        StartCoroutine(PopTimedChatMessage());
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
        
        scrollRect.verticalNormalizedPosition = 0f;
    }

    private IEnumerator PopTimedChatMessage()
    {
        while (true)
        {
            if (Time.timeScale != 0)
            {
                SendMessageToChat("questo messaggio è nella tua testa ahahahahah. sono passati " + Time.time + " secondi");
            }

            yield return new WaitForSeconds(timerChat);
        }
    }
}

[System.Serializable]
public class Message
{
    public string text;
    public TMP_Text textObject;
}
