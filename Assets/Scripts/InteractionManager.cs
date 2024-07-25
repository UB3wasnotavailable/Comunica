using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    // Array dei GameObject che devono apparire
    public GameObject[] dialogueBoxes;
    public int dialogueIndex;

    private void Start()
    {
        dialogueIndex = 0;
    }

    // Metodo per abilitare il GameObject corretto
    public void ActivateDialogueBox(int index)
    {
        if (index >= 0 && index < dialogueBoxes.Length)
        {
            dialogueBoxes[index].SetActive(true);
        }
        else
        {
            Debug.LogWarning("Indice fuori dai limiti dell'array di dialogueBoxes.");
        }
    }
}
