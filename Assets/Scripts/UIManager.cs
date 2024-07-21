using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject controlsPanel;
    public GameObject contactsPanel;
    public GameObject inGameMenuPanel;
    public GameObject winMenuPanel;
    public GameObject loseMenuPanel;
    public GameObject GameOverPanel;
    public GameObject ChatPanel;
    public GameObject grayChatterMenuPanel;

    public GameObject player;
    private PlayerController playerController;
    public GameManager GM;
    public Camera mainCamera;
    private Stack<GameObject> menuHistory = new Stack<GameObject>();

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

    public void ShowMainMenu()
    {
        menuHistory.Clear();

        ChatPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        controlsPanel.SetActive(false);
        contactsPanel.SetActive(false);
        inGameMenuPanel.SetActive(false);
        winMenuPanel.SetActive(false);
        loseMenuPanel.SetActive(false);
        GameOverPanel.SetActive(false);
        grayChatterMenuPanel.SetActive(false);
    }

    public void StartGame()
    {
        mainMenuPanel.SetActive(false);
        ChatPanel.SetActive(true);
        Time.timeScale = 1;
        GM.StartLevel(GM.currentLevelIndex);
    }

    public void ShowControls()
    {
        PushCurrentMenu();
        mainMenuPanel.SetActive(false);
        inGameMenuPanel.SetActive(false);
        controlsPanel.SetActive(true);
    }

    public void ShowContacts()
    {
        PushCurrentMenu();
        mainMenuPanel.SetActive(false);
        contactsPanel.SetActive(true);
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
    }

    public void HideInGameMenu()
    {
        inGameMenuPanel.SetActive(false);
        Time.timeScale = 1; // Resume the game
    }

    public void RestartLevel()
    {
        PushCurrentMenu();
        //TeleportPlayerToLevel(currentLevel);
        GM.ResetLevel();
        HideInGameMenu();
    }

    public void NextLevelButton()
    {
        winMenuPanel.SetActive(false);
        GM.NextLevel();
    }

    public void ShowWinMenu()
    {
        PushCurrentMenu();
        winMenuPanel.SetActive(true);
        Time.timeScale = 0; // Pause the game
    }

    public void ShowLoseMenu()
    {
        PushCurrentMenu();
        loseMenuPanel.SetActive(true);
        Time.timeScale = 0; // Pause the game
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
