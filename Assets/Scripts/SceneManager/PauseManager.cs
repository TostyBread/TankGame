using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI; // Drag your Pause Menu Panel here
    private bool isPaused = false; // Condition to make sure if the game is paused
    SceneLoader loader;

    private void Start()
    {
        loader = GetComponent<SceneLoader>(); // Reference Sceneloader
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
        loader.RestartGame();
    }

    public void QuitToMainMenu() //Return to main menu
    {
        loader.ReturnMainMenu();
    }
}
