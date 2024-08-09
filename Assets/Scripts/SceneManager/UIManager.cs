using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject gameOverPanel;
    public GameObject victoryPanel;
    public GameObject playerStatus;
    public TextMeshProUGUI timerText; // Reference to the UI Text component for the timer

    private float timer;
    private bool isTimerRunning;

    private void Start()
    {
        // Ensure panels are hidden at the start
        gameOverPanel.SetActive(false);
        victoryPanel.SetActive(false);

        // Initialize timer
        timer = 0f;
        isTimerRunning = true;
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
        if (playerStatus == null)
        {
            ShowGameOver();
        }
    }

    private void UpdateTimerText()
    {
        // Format the timer as minutes:seconds
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);
        timerText.text = $"You've defeated the enemy under {minutes:D2}:{seconds:D2}!";
    }

    public void ShowGameOver()
    {
        isTimerRunning = false; // Stop the timer
        gameOverPanel.SetActive(true);
    }

    public void ShowVictory()
    {
        isTimerRunning = false; // Stop the timer
        victoryPanel.SetActive(true);
    }
}
