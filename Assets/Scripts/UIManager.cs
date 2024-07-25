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

    public GameObject player;
    private PlayerController playerController;
    public GameManager GM;
    public Camera mainCamera;
    private Stack<GameObject> menuHistory = new Stack<GameObject>();
    
    public int currentButtonIndex = 0;
    public Button[] mainMenuButtons;
    public Button[] controlsMenuButtons;
    public Button[] contactsMenuButtons;
    public Button[] inGameMenuButtons;
    public Button[] winMenuButtons;
    public Button[] loseMenuButtons;
    public Button[] gameOverMenuButtons;
    public Button[] feedbackMenuButtons;
    public Button[] grayChatterMenuButtons;

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
        ShowMainMenu();
    }
    
    void Update()
    {
        HandleMenuNavigation();
    }
    
    void HandleMenuNavigation()
    {
        Button[] buttons = GetCurrentMenuButtons();
        if (buttons.Length == 0) return;
        
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentButtonIndex--;
            if (currentButtonIndex < 0) currentButtonIndex = GetCurrentMenuButtons().Length - 1;
            HighlightButton(GetCurrentMenuButtons()[currentButtonIndex]);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentButtonIndex++;
            if (currentButtonIndex >= GetCurrentMenuButtons().Length) currentButtonIndex = 0;
            HighlightButton(GetCurrentMenuButtons()[currentButtonIndex]);
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            GetCurrentMenuButtons()[currentButtonIndex].onClick.Invoke();
        }
    }
    
    void HighlightButton(Button button)
    {
        EventSystem.current.SetSelectedGameObject(button.gameObject);
    }
    
    void HighlightFirstButton(Button[] buttons)
    {
        if (buttons.Length > 0)
        {
            currentButtonIndex = 0;
            HighlightButton(buttons[currentButtonIndex]);
        }
    }
    
    Button[] GetCurrentMenuButtons()
    {
        if (mainMenuPanel.activeSelf) return mainMenuButtons;
        if (controlsPanel.activeSelf) return controlsMenuButtons;
        if (contactsPanel.activeSelf) return contactsMenuButtons;
        if (inGameMenuPanel.activeSelf) return inGameMenuButtons;
        if (winMenuPanel.activeSelf) return winMenuButtons;
        if (loseMenuPanel.activeSelf) return loseMenuButtons;
        if (grayChatterMenuPanel.activeSelf) return grayChatterMenuButtons;
        return new Button[0];
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

        HighlightFirstButton(mainMenuButtons);
    }

    public void StartGame()
    {
        mainMenuPanel.SetActive(false);
        Time.timeScale = 1;
        GM.StartLevel(GM.currentLevelIndex);
        playerController.isSpeedAdjusted = false;
        chatPanel.SetActive(true);
    }

    public void ShowControls()
    {
        PushCurrentMenu();
        mainMenuPanel.SetActive(false);
        inGameMenuPanel.SetActive(false);
        controlsPanel.SetActive(true);
        chatPanel.SetActive(false);
        
        HighlightFirstButton(controlsMenuButtons);
    }

    public void ShowContacts()
    {
        PushCurrentMenu();
        mainMenuPanel.SetActive(false);
        contactsPanel.SetActive(true);
        
        HighlightFirstButton(contactsMenuButtons);
    }

    public void GoBackToMainMenu()
    {
        PushCurrentMenu();
        GM.currentLevelIndex = 0;
        ShowMainMenu();
    }

    public void ShowInGameMenu()
    {
        PushCurrentMenu();
        inGameMenuPanel.SetActive(true);
        Time.timeScale = 0; // Pause the game
        HighlightFirstButton(inGameMenuButtons);
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
        HighlightFirstButton(winMenuButtons);
    }

    public void ShowLoseMenu()
    {
        PushCurrentMenu();
        loseMenuPanel.SetActive(true);
        Time.timeScale = 0; // Pause the game
        HighlightFirstButton(loseMenuButtons);
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
    }
    
    public void ShowGrayChatterMenu()
    {
        PushCurrentMenu();
        grayChatterMenuPanel.SetActive(true);
        Time.timeScale = 0; // Pause the game
        HighlightFirstButton(grayChatterMenuButtons);
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
    }
}
