using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject gameOverPanel;
    public GameObject victoryPanel;
    public GameObject defaultTankStatus;
    public GameObject specialTankStatus;
    public TextMeshProUGUI timerText; // Reference to the UI Text component for the timer

    private float timer;
    public bool isTimerRunning = true;

    private void Start()
    {
        // Ensure panels are hidden at the start
        gameOverPanel.SetActive(false);
        victoryPanel.SetActive(false);

        // Initialize timer
        timer = 0f;
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            timer += Time.deltaTime;
            UpdateTimerText();
        }
    }

    private void FixedUpdate()
    {
        // Check game over conditions here, if applicable
        if (defaultTankStatus == null || specialTankStatus == null)
        {
            if (isTimerRunning) // Ensure it only triggers once
            {
                ShowGameOver();
            }
        }
    }

    public void UpdateTimerText()
    {
        // Format the timer as minutes:seconds
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);
        timerText.text = $"You've defeated the enemy under {minutes:D2}:{seconds:D2}!";
    }

    public void ShowGameOver()
    {
        if (!isTimerRunning) return; // Prevent multiple calls

        isTimerRunning = false; // Stop the timer
        UpdateTimerText();      // Ensure the timer text is up-to-date
        gameOverPanel.SetActive(true);
    }

}

