using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject gameOverPanel; // game over game object
    public GameObject victoryPanel; // vitory game object
    public GameObject defaultTankStatus; // default tank game object
    public GameObject specialTankStatus; // special tank game object
    public TextMeshProUGUI timerText; // Reference to the UI Text component for the timer

    public AudioSource audioSource; // Reference to the AudioSource component
    public AudioClip gameOverSound; // Reference to the Game Over sound clip

    private float timer;
    public bool isTimerRunning = true;

    private void Start()
    {
        // Ensure panels are hidden at the start
        gameOverPanel.SetActive(false);
        victoryPanel.SetActive(false);

        // Initialize timer
        timer = 0f;

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>(); // Reference AudioSource if not assigned in the Inspector
        }
    }

    private void Update()
    {
        if (isTimerRunning) // if timer still running isnt false
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
        PlaySound(gameOverSound); // Play game over sound
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource && clip)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}