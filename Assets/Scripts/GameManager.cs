using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerController player;
    public UIManager ui;

    public List<Level> levels;
    public int currentLevelIndex = 0;
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        ui = GameObject.FindWithTag("UI").GetComponent<UIManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            ui.ShowInGameMenu();
        }
    }
    
    public void StartLevel(int levelIndex)
    {
        Level level = levels[levelIndex];

        // Move player to spawn position
        player.transform.position = level.spawnPoint.position;
        player.TeleportTo(level.spawnPoint.position, level.spawnPoint.rotation);
        player.speed = 0;
        player.isChatting = false;

        foreach (Level lvl in levels)
        {
            lvl.levelCamera.SetActive(false);
        }
        level.levelCamera.SetActive(true);

        Chatter[] chatters = FindObjectsOfType<Chatter>();
        foreach (Chatter chatter in chatters)
        {
            chatter.ResetChatter();
        }
    }
    
    public void NextLevel()
    {
        if (currentLevelIndex < levels.Count - 1)
        {
            currentLevelIndex++;
            StartLevel(currentLevelIndex);
            Time.timeScale = 1;
        }
        else
        {
            currentLevelIndex = 0;
            Time.timeScale = 0;
            ui.ShowFeedbackMenu();
        }
    }
    
    public void ResetLevel()
    {
        StartLevel(currentLevelIndex);
    }
}
