using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject controlsPanel;
    public GameObject contactsPanel;
    public GameObject inGameMenuPanel;
    public GameObject winMenuPanel;
    public GameObject loseMenuPanel;
    public GameObject GameOverPanel;
    public GameObject grayChatterMenuPanel;
    public GameObject feedbackMenuPanel;
    public GameObject chatPanel;
    public GameObject levelSelectorPanel;

    public GameObject player;
    private PlayerController playerController;
    public GameManager GM;
    public Camera mainCamera;
    private Stack<GameObject> menuHistory = new Stack<GameObject>();
    
    public GameObject selectionPointer;
    private EventSystem eventSystem;
   
    public int currentButtonIndex = 0;
    public Button mainMenuButton;
    public Button controlsMenuButton;
    public Button contactsMenuButton;
    public Button inGameMenuButton;
    public Button winMenuButton;
    public Button loseMenuButton;
    public Button gameOverMenuButton;
    public Button feedbackMenuButton;
    public Button grayChatterMenuButton;
    public Button levelSelectorButton;

    private Vector3[] levelPositions = new Vector3[] {
        new Vector3(0, 0, 0),
        new Vector3(10, 0, 0), // Add positions for each level
        new Vector3(20, 0, 0)
    };

    private int currentLevel = 0;

    void Start()
    {
        Time.timeScale = 0;
        player = GameObject.FindWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        GM = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        eventSystem = EventSystem.current;
        ShowMainMenu();
    }
    
    private void SetSelectedButton(Button button)
    {
        eventSystem.SetSelectedGameObject(null); // Deselect any currently selected object
        eventSystem.SetSelectedGameObject(button.gameObject); // Select the new button
    }
    
    public void ShowMainMenu()
    {
        menuHistory.Clear();
        
        mainMenuPanel.SetActive(true);
        controlsPanel.SetActive(false);
        contactsPanel.SetActive(false);
        inGameMenuPanel.SetActive(false);
        winMenuPanel.SetActive(false);
        loseMenuPanel.SetActive(false);
        GameOverPanel.SetActive(false);
        grayChatterMenuPanel.SetActive(false);
        chatPanel.SetActive(false);
        levelSelectorPanel.SetActive(false);
        SetSelectedButton(mainMenuButton);

        Debug.Log("entrato in main menu");

        //HighlightFirstButton(mainMenuButtons);
    }

    public void StartGame()
    {
        mainMenuPanel.SetActive(false);
        Time.timeScale = 1;
        GM.StartLevel(GM.currentLevelIndex);
        playerController.isSpeedAdjusted = false;
        chatPanel.SetActive(true);
        Debug.Log("starting game");
    }

    public void ShowControls()
    {
        PushCurrentMenu();
        inGameMenuPanel.SetActive(false);
        controlsPanel.SetActive(true);
        chatPanel.SetActive(false);
        mainMenuPanel.SetActive(false);
        SetSelectedButton(controlsMenuButton);

        Debug.Log("entrato in controls");
        
        //HighlightFirstButton(controlsMenuButtons);
    }

    public void ShowContacts()
    {
        PushCurrentMenu();
        mainMenuPanel.SetActive(false);
        contactsPanel.SetActive(true);
        SetSelectedButton(contactsMenuButton);
        //HighlightFirstButton(contactsMenuButtons);
    }

    public void ShowLevelSelector()
    {
        PushCurrentMenu();
        mainMenuPanel.SetActive(false);
        inGameMenuPanel.SetActive(false);
        winMenuPanel.SetActive(false);
        loseMenuPanel.SetActive(false);
        levelSelectorPanel.SetActive(true);
        SetSelectedButton(levelSelectorButton);
    }
    
    public void SelectLevel(int levelNumber)
    {
        levelSelectorPanel.SetActive(false);
        GM.currentLevelIndex = (levelNumber - 1);
        GM.StartLevel(levelNumber - 1);
        Time.timeScale = 1;
    }

    public void GoBackToMainMenu()
    {
        PushCurrentMenu();
        GM.currentLevelIndex = 0;
        ShowMainMenu();
        Debug.Log("cerco di tornare al main menu");
    }

    public void ShowInGameMenu()
    {
        PushCurrentMenu();
        inGameMenuPanel.SetActive(true);
        Time.timeScale = 0; // Pause the game
        SetSelectedButton(inGameMenuButton);

        //HighlightFirstButton(inGameMenuButtons);
    }

    public void HideInGameMenu()
    {
        inGameMenuPanel.SetActive(false);
        loseMenuPanel.SetActive(false);
        Time.timeScale = 1; // Resume the game
    }

    public void RestartLevel()
    {
        PushCurrentMenu();
        //TeleportPlayerToLevel(currentLevel);
        GM.ResetLevel();
        HideInGameMenu();
        playerController.currentSpeed = 0;
        playerController.isSpeedAdjusted = false;
    }

    public void NextLevelButton()
    {
        winMenuPanel.SetActive(false);
        GM.NextLevel();
        playerController.isSpeedAdjusted = false;
    }

    public void ShowWinMenu()
    {
        PushCurrentMenu();
        winMenuPanel.SetActive(true);
        Time.timeScale = 0; // Pause the game
        SetSelectedButton(winMenuButton);
        //HighlightFirstButton(winMenuButtons);
    }

    public void ShowLoseMenu()
    {
        PushCurrentMenu();
        loseMenuPanel.SetActive(true);
        Time.timeScale = 0; // Pause the game
        SetSelectedButton(loseMenuButton);

        //HighlightFirstButton(loseMenuButtons);
    }

    private void TeleportPlayerToLevel(int levelIndex)
    {
        player.transform.position = levelPositions[levelIndex];
        mainCamera.transform.position = new Vector3(levelPositions[levelIndex].x, mainCamera.transform.position.y, levelPositions[levelIndex].z);
        currentLevel = levelIndex;
        Time.timeScale = 1; // Resume the game
    }

    public void ShowFeedbackMenu()
    {
        PushCurrentMenu();
        GameOverPanel.SetActive(true);
        SetSelectedButton(feedbackMenuButton);
    }
    
    public void ShowGrayChatterMenu()
    {
        Debug.Log("entrato dentro il menu chatter grigio");
        PushCurrentMenu();
        grayChatterMenuPanel.SetActive(true);
        Time.timeScale = 0; // Pause the game
        SetSelectedButton(grayChatterMenuButton);
        //HighlightFirstButton(grayChatterMenuButtons);
    }

    public void HideGrayChatterMenu()
    {
        grayChatterMenuPanel.SetActive(false);
        Time.timeScale = 1; // Resume the game
    }
    
    public void GrayChatterOptionIgnore()
    {
        playerController.GrayChatterOptionIgnore();
    }

    public void GrayChatterOptionInteract()
    {
        playerController.GrayChatterOptionInteract();
    }
    
    public void BackButton()
    {
        if (menuHistory.Count > 0)
        {
            GameObject previousMenu = menuHistory.Pop();
            HideAllMenus();
            previousMenu.SetActive(true);
            Button previousMenuButton = previousMenu.GetComponentInChildren<Button>();
            SetSelectedButton(previousMenuButton);
            Debug.Log("cerco di tornare indietro");
        }
        else
        {
            ShowMainMenu();
        }
    }
    
    private void PushCurrentMenu()
    {
        GameObject activeMenu = GetActiveMenu();
        if (activeMenu != null)
        {
            menuHistory.Push(activeMenu);
        }
    }
    
    private GameObject GetActiveMenu()
    {
        if (mainMenuPanel.activeSelf) return mainMenuPanel;
        if (controlsPanel.activeSelf) return controlsPanel;
        if (contactsPanel.activeSelf) return contactsPanel;
        if (inGameMenuPanel.activeSelf) return inGameMenuPanel;
        if (winMenuPanel.activeSelf) return winMenuPanel;
        if (loseMenuPanel.activeSelf) return loseMenuPanel;
        if (grayChatterMenuPanel.activeSelf) return grayChatterMenuPanel;
        if (levelSelectorPanel.activeSelf) return levelSelectorPanel;
        return null;
    }
    
    private void HideAllMenus()
    {
        mainMenuPanel.SetActive(false);
        controlsPanel.SetActive(false);
        contactsPanel.SetActive(false);
        inGameMenuPanel.SetActive(false);
        winMenuPanel.SetActive(false);
        loseMenuPanel.SetActive(false);
        grayChatterMenuPanel.SetActive(false);
        levelSelectorPanel.SetActive(false);
    }
}
