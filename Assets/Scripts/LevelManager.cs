using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using Zenject;

public class LevelManager : MonoBehaviour
{
    public string levelsDirectory = "Levels"; // Directory inside Resources folder
    public TextMeshProUGUI levelNumberLabel;
    Level _levelData;

    public const int levelsNumber = 5;
    public int levelIndex = 1;
    public int currentLevelIndex;

    public int NumberOfMatchs = 0;

    //Pause/Resume
    public GameObject pausePanel;
    private bool isPaused = false;

    private GameObject slotPrefab;
    private GameObject itemPrefab;
    private GameObject currentLevel;
    private LevelGenerator levelGenerator;
    private LevelDataLoader levelDataLoader;
    private PopUpSystem popUpSystem;
    private MatchFeedback matchFeedback;
    public LevelTimer levelTimer;

    [Inject]
    public void Construct(
        [Inject(Id = "SlotPrefab")] GameObject slotPrefab,
        [Inject(Id = "ItemPrefab")] GameObject itemPrefab,
        PopUpSystem popUpSystem,
        MatchFeedback matchFeedback,
        LevelTimer levelTimer,
        LevelDataLoader levelDataLoader)
    {
        this.slotPrefab = slotPrefab;
        this.itemPrefab = itemPrefab;
        this.popUpSystem = popUpSystem;
        this.matchFeedback = matchFeedback;
        this.levelTimer = levelTimer;
        this.levelDataLoader = levelDataLoader;
    }




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


    // Method to load level data and generate the level
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

        // Load the new level using LevelDataLoader
        string jsonFileName = "Level_" + _levelIndex;
        string fullPath = Path.Combine(levelsDirectory, jsonFileName);
        LoadLevel(fullPath);

        // Generate and store the new level
        currentLevel = levelGenerator.GenerateLevel(slotPrefab, itemPrefab);
        _levelData = levelGenerator.levelData;

        // Set timer based on the level data
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

    // Method to load level data through LevelDataLoader
    void LoadLevel(string jsonPath)
    {
        Level loadedLevel = levelDataLoader.LoadLevelData(jsonPath);

        if (loadedLevel != null)
        {
            levelGenerator.SetLevelData(JsonUtility.ToJson(loadedLevel), slotPrefab, itemPrefab);
        }
        else
        {
            Debug.LogError("Failed to load level data from: " + jsonPath);
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
