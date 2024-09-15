using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System;

public class LevelTimer : MonoBehaviour
{
    public float levelTime; // Total time in (seconds)
    public TextMeshProUGUI timerText;
    public Color lowTimeColor = Color.red;
    public float lowTimeThreshold = 30f;
    public float sizePulseSpeed = 0.35f;
    public float sizePulseAmount = 0.15f;
    public bool isPaused = false;

    private float timeRemaining;
    private bool isTimeLow = false;
    private Vector3 originalTextScale;
    private Coroutine timerCoroutine;  
    private Coroutine pulseCoroutine;  

    public event Action OnTimesUp;

    public void SetLevelTime(float _levelTime)
    {
        levelTime = _levelTime;
    }

    public void StartTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine); // Stop any previous coroutine
        }

        timeRemaining = levelTime;
        originalTextScale = timerText.transform.localScale;
        UpdateTimerDisplay(); 
        isPaused = false;

        // Start the new timer coroutine
        timerCoroutine = StartCoroutine(TimerRoutine());
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

    private IEnumerator TimerRoutine()
    {
        while (timeRemaining > 0)
        {
            if (!isPaused)
            {
                timeRemaining -= Time.deltaTime;
                // Start low-time effects (if) below threshold
                if (timeRemaining <= lowTimeThreshold && !isTimeLow)
                {
                    StartLowTimeEffects();
                }

                // If time runs out
                if (timeRemaining <= 0)
                {
                    timeRemaining = 0;
                    HandleLevelLost();
                }
                UpdateTimerDisplay();
            }
            yield return null; 
        }
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void StartLowTimeEffects()
    {
        isTimeLow = true;
        timerText.color = lowTimeColor;

        if (pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);  // Stop any existing pulse effect
        }
        pulseCoroutine = StartCoroutine(LowTimePulse());
    }

    private IEnumerator LowTimePulse()
    {
        while (timeRemaining > 0 && isTimeLow)
        {
            float scaleAmount = 1f + Mathf.PingPong(Time.time * sizePulseSpeed, sizePulseAmount);
            timerText.transform.localScale = originalTextScale * scaleAmount;
            yield return null;
        }
    }

    private void HandleLevelLost()
    {
        Debug.Log("Level Lost! Time's up.");
        OnTimesUp?.Invoke();

        // Stop the timer coroutine
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }
    }

    private void StopLowTimeEffects()
    {
        isTimeLow = false;

        // Stop the pulsing effect if running
        if (pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
            pulseCoroutine = null;
        }

        timerText.color = Color.white;  // Reset text color
        timerText.transform.localScale = originalTextScale;  // Reset text scale
    }
}
