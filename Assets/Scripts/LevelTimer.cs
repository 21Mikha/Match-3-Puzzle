using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class LevelTimer : MonoBehaviour
{
    public float levelTime = 120f; // Total time in seconds
    public TextMeshProUGUI timerText;         // Reference to the UI Text component
    public Color lowTimeColor = Color.red; // Color for when time is low
    public float lowTimeThreshold = 30f;   // Threshold for "low time" effects
    public float pulseSpeed = 1f;  // Speed of pulsing when time is low
    public bool isPaused = false;  // Timer pause state

    private float timeRemaining;
    private bool isTimeLow = false;  // Track if time is considered "low"
    private Color originalTextColor; // Store original color of the text

    void Start()
    {
        // Initialize the timer with the total time and the original text color
        timeRemaining = levelTime;
        originalTextColor = timerText.color;
        UpdateTimerDisplay();
        StartCoroutine(TimerRoutine());
    }

    IEnumerator TimerRoutine()
    {
        while (timeRemaining > 0)
        {
            if (!isPaused)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerDisplay();

                if (timeRemaining <= lowTimeThreshold && !isTimeLow)
                {
                    StartLowTimeEffects();
                }

                // Check if time has run out
                if (timeRemaining <= 0)
                {
                    timeRemaining = 0;
                    HandleLevelLost();
                }
            }
            yield return null;
        }
    }

    void UpdateTimerDisplay()
    {
        // Convert remaining time to minutes and seconds
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);

        // Update the UI text with the formatted time
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void StartLowTimeEffects()
    {
        isTimeLow = true;
        StartCoroutine(LowTimePulse());
    }

    IEnumerator LowTimePulse()
    {
        while (timeRemaining > 0 && isTimeLow)
        {
            // Pulse the text color between normal and low time color
            float t = Mathf.PingPong(Time.time * pulseSpeed, 1f);
            timerText.color = Color.Lerp(originalTextColor, lowTimeColor, t);

            yield return null;
        }
    }

    void HandleLevelLost()
    {
        // Stop the timer and trigger the lost level condition
        Debug.Log("Level Lost! Time's up.");
        StopCoroutine(TimerRoutine());
        // Trigger game over logic or UI
        // Example: GameManager.Instance.OnLevelLost();
    }

    public void PauseTimer()
    {
        isPaused = true;
    }

    public void ResumeTimer()
    {
        isPaused = false;
    }

    public void AddTime(float additionalTime)
    {
        timeRemaining += additionalTime;
        if (timeRemaining > lowTimeThreshold && isTimeLow)
        {
            StopLowTimeEffects();
        }
    }

    void StopLowTimeEffects()
    {
        isTimeLow = false;
        StopCoroutine(LowTimePulse());
        timerText.color = originalTextColor;
    }
}
