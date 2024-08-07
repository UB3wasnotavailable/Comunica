using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelector : MonoBehaviour
{
    private UIManager ui;

    void Start()
    {
        // Get the GameManager instance
        ui = FindObjectOfType<UIManager>();

        // Find all buttons that are children of this game object
        Button[] levelButtons = GetComponentsInChildren<Button>();

        foreach (Button button in levelButtons)
        {
            if (button.name.StartsWith("lv"))
            {
                // Find the TextMeshProUGUI component in the button's children
                TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            
                // Extract the level number from the button text
                string levelNumberString = buttonText.text;
                Debug.Log("il numero di livello è " + buttonText.text);
                if (int.TryParse(levelNumberString, out int levelNumber))
                {
                    // Set up the button's onClick listener
                    button.onClick.AddListener(() => ui.SelectLevel(levelNumber));
                    Debug.Log("il setup vuole che il bottone sia " + levelNumber);
                }
            }
        }
    }
}
