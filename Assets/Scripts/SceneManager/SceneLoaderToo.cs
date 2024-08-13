using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // For UI handling

public class SceneLoaderToo : MonoBehaviour
{
    public string level1SceneName = "Level1"; // Name of the Level1 scene
    public string briefingSceneName = "Briefing"; // Name of the Briefing scene
    public Button proceedButton; // Reference to the button to start the game

    private void Start()
    {
        proceedButton.gameObject.SetActive(true); // Show the button at the start
        proceedButton.onClick.AddListener(StartGame); // Add a listener to the button click event
    }

    // Call this function to transition to Level1 scene
    public void StartGame()
    {
        // Load Level1 scene and unload the Briefing scene
        SceneManager.LoadScene(level1SceneName, LoadSceneMode.Additive);

        // Ensure the Briefing scene is unloaded
        SceneManager.UnloadSceneAsync(briefingSceneName);
    }
}