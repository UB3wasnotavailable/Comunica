using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public GameObject[] dialogueBoxes;
    public int dialogueIndex;
    // Start is called before the first frame update
    private void Start()
    {
        dialogueIndex = 0;
    }

    public void ActivateDialogueBox(int index)
    {
        if (index >= 0 && index < dialogueBoxes.Length)
        {
            dialogueBoxes[index].SetActive(true);
        }
    }

    public void ResetBoxes()
    {
        foreach (var GameObject in dialogueBoxes)
        {
            GameObject.SetActive(false);
        }
    }
}
