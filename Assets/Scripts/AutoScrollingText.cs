using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AutoScrollingText : MonoBehaviour
{
    public GameObject DialogueTextPrefab; // Prefab del testo
    public Transform content; // Contenitore per il testo
    public ScrollRect scrollRect; // Componente Scroll Rect

    public List<string> messages; // Lista di messaggi da visualizzare
    private int currentMessageIndex = 0; // Indice del messaggio corrente

    public Font customFont; // Assegna il tuo font in Inspector

    private float scrollSpeed = 20f; // Velocità dello scorrimento

    void Update()
    {
        if (content != null && scrollRect != null)
        {
            // Scorrere il contenuto
            content.transform.Translate(Vector3.down * scrollSpeed * Time.deltaTime);

            // Controlla se il contenuto è fuori dalla vista
            if (content.GetComponent<RectTransform>().anchoredPosition.y > 0)
            {
                content.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            }
        }
    }

    public void DisplayNextMessage()
    {
        if (messages == null || messages.Count == 0)
        {
            Debug.LogWarning("Nessun messaggio da visualizzare.");
            return;
        }

        if (DialogueTextPrefab == null || content == null)
        {
            Debug.LogError("Prefab del testo o contenitore non assegnato!");
            return;
        }

        if (currentMessageIndex >= messages.Count)
        {
            Debug.Log("Non ci sono più messaggi da visualizzare.");
            return;
        }

        // Crea e visualizza un nuovo messaggio
        string message = messages[currentMessageIndex];
        GameObject newText = Instantiate(DialogueTextPrefab, content);
        Text textComponent = newText.GetComponent<Text>();

        if (textComponent != null)
        {
            textComponent.text = message;
            textComponent.font = customFont; // Cambia il font qui
        }
        else
        {
            Debug.LogError("Il prefab del testo non contiene un componente Text.");
        }

        currentMessageIndex++;

        // Aggiorna la posizione dello scroll
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
        Canvas.ForceUpdateCanvases();

        // Distruggi il testo dopo un certo periodo (opzionale)
        Destroy(newText, 10f); // Regola a tuo piacimento
    }

    public void TriggerDisplayMessage()
    {
        if (currentMessageIndex < messages.Count)
        {
            DisplayNextMessage();
        }
        else
        {
            Debug.Log("Non ci sono più messaggi da visualizzare.");
        }
    }

    public void ResetMessages()
    {
        currentMessageIndex = 0;
    }
}
