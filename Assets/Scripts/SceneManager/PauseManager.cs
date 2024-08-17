using UnityEngine.SceneManagement;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI; // Drag your Pause Menu Panel here
    private bool isPaused = false;

    private void Start()
    {
        pauseMenuUI.SetActive(false); // Hide Menu UI at start
        //Make sure the game isn't in pause state everytime you enter a new scene
        isPaused = false;
        Resume();
    }
    void Update()
    {
        // Toggle pause menu visibility when the player presses the "Escape" key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false); // Hide the pause menu
        Time.timeScale = 1f; // Resume the game
        isPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true); // Show the pause menu
        Time.timeScale = 0f; // Pause the game
        isPaused = true;
    }

    public void RestartGame() // Reloads current scene
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public void QuitToMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); // Return to main menu
    }
}
