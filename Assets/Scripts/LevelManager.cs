using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public GameObject SlotPrefab;
    public GameObject ItemPrefab;

    private GameObject currentLevel;

    public string levelsDirectory = "Levels"; // Directory inside Resources folder
    public TextMeshProUGUI levelNumberLabel;
    public LevelTimer levelTimer;

    public PopUpSystem popUpSystem;

    private LevelGenerator levelGenerator;
    public MatchFeedback matchFeedback;
    Level _levelData;

    public const int levelsNumber = 5;
    public int levelIndex = 1;
    public int currentLevelIndex;
    public int NumberOfMatchs = 0;

    //Pause/Resume
    public GameObject pausePanel;
    private bool isPaused = false;


    void Start()
    {
        levelGenerator = GetComponent<LevelGenerator>();
        levelTimer.OnTimesUp += HandleOnTimesUp;

        // Start by loading the first level
        Debug.Log("Loading level...");
        LoadLevelByIndex(levelIndex);
        currentLevelIndex = levelIndex;
        currentLevel.SetActive(true);
        levelTimer.StartTimer();
    }

    //Function to pause the game and show the pause menu
    public void PauseGame()
    {
        if (isPaused) return;

        isPaused = true;
        Time.timeScale = 0;

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }
        Debug.Log("Game Paused");
    }

    //Function to resume the game and hide the pause menu
    public void ResumeGame()
    {
        if (!isPaused) return;

        isPaused = false;
        Time.timeScale = 1;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        Debug.Log("Game Resumed");
    }

    public void HandleAfterTransition()
    {
        if (currentLevel != null)
        {
            currentLevel.SetActive(true);
            levelTimer.StartTimer(); // Start the timer when the level becomes active
        }
    }

    //Main Method to handle losing level logic
    void HandleOnTimesUp()
    {
        Debug.Log("Time's Up! Level Lost.");
        NumberOfMatchs = 0;
        popUpSystem.ShowLosingPopUp();
    }

    
    //Main method to load level and store its data in GameObject (currentLevel)
    public void LoadLevelByIndex(int _levelIndex)
    {
        levelIndex = _levelIndex;
        currentLevelIndex = levelIndex;

        // Destroy the current level if it exists
        if (currentLevel != null)
        {
            Destroy(currentLevel);
        }

        // Update the level label
        levelNumberLabel.text = "Level " + _levelIndex;

        // Load the new level based on the index
        string jsonFileName = "Level_" + _levelIndex;
        string fullPath = Path.Combine(levelsDirectory, jsonFileName);
        LoadLevel(fullPath);

        // Generate and store the new level
        currentLevel = levelGenerator.GenerateLevel(SlotPrefab, ItemPrefab);
        _levelData = levelGenerator.levelData;

        //Set timer based on the level data
        levelTimer.SetLevelTime(_levelData.time);

        foreach (Transform child in currentLevel.transform)
        {
            Slot slotComponent = child.GetComponent<Slot>();
            if (slotComponent != null)
            {
                slotComponent.OnMatch += HandleOnMatch;
            }
        }

        currentLevel.SetActive(false);  // Initially set inactive until after transition
    }

    //Method to pass json file -> LevelGenerator
    void LoadLevel(string jsonPath)
    {
        TextAsset jsonTextFile = Resources.Load<TextAsset>(jsonPath);

        if (jsonTextFile != null)
        {
            levelGenerator.SetLevelData(jsonTextFile.text, SlotPrefab, ItemPrefab);
        }
        else
        {
            Debug.LogError("JSON file not found at: " + jsonPath);
        }
    }

    public void RetryLevel()
    {
        // Retry the current level
        LoadLevelByIndex(currentLevelIndex);
    }

    void HandleOnMatch(Slot slot)
    {
        NumberOfMatchs++;
        if (CheckWinning())
        {
            if (currentLevelIndex != levelsNumber)// if not last level
            {
                NumberOfMatchs = 0;
                levelTimer.PauseTimer(); // Pause the timer when winning
                popUpSystem.ShowWinningPopUp();
            }

            else
            {
                levelTimer.PauseTimer();
                // Destroy the current level if it exists
                if (currentLevel != null)
                {
                    Destroy(currentLevel);
                }
                popUpSystem.ShowFinishGamePopUp();
            }

        }
        matchFeedback.PlayFeedback(slot);
    }

    bool CheckWinning()
    {
        if (NumberOfMatchs == _levelData.itemsNumber)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
