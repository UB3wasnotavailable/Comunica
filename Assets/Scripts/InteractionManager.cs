using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    // Array dei GameObject che devono apparire
    public GameObject[] dialogueBoxes;
    public int dialogueIndex;

    // Metodo per abilitare il GameObject corretto
    public void ActivateDialogueBox(int index)
    {
        if (index >= 0 && index < dialogueBoxes.Length)
        {
            // Disabilita tutti i box dialogo
           
            // Abilita solo il box dialogo richiesto
            dialogueBoxes[index].SetActive(true);
        }
        else
        {
            Debug.LogWarning("Indice fuori dai limiti dell'array di dialogueBoxes.");
        }
    }
}
