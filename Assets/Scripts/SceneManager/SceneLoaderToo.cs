using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // For UI handling
using System.Collections;

public class SceneLoaderToo : MonoBehaviour
{
    public string level1SceneName = "Level1"; // Name of the Level1 scene
    public string briefingSceneName = "Briefing"; // Name of the Briefing scene
    public Button proceedButton; // Reference to the button to start the game
    private AsyncOperation asyncLoad; // To hold the async operation for later use

    private void Start()
    {
        proceedButton.gameObject.SetActive(false); // Make sure the button is hidden at the start
        StartCoroutine(LoadLevel1Async()); // Start preloading Level1
    }

    private IEnumerator LoadLevel1Async()
    {
        // Start loading Level1 scene asynchronously
        asyncLoad = SceneManager.LoadSceneAsync(level1SceneName, LoadSceneMode.Additive);
        asyncLoad.allowSceneActivation = false; // Prevent the scene from activating immediately

        // Wait until Level1 is fully loaded
        while (!asyncLoad.isDone)
        {
            //Debug.Log(asyncLoad.progress);
            yield return null; // Wait until the next frame

            if (asyncLoad.progress >= 0.9f)
            {
                //Debug.Log("Level1 loaded. Click the button to start.");
                proceedButton.gameObject.SetActive(true); // Show the button
                //Debug.Log("Button should now be active."); // Confirmation log
            }
        }
    }

    // Call this function to transition to Level1 scene
    public void StartGame()
    {
        // Activate the Level1 scene when the button is clicked
        asyncLoad.allowSceneActivation = true; // Allow Level1 to activate

        // Start unloading Briefing scene with a delay
        StartCoroutine(UnloadBriefingScene());
    }

    private IEnumerator UnloadBriefingScene()
    {
        // Wait for a frame to ensure Level1 is fully active
        yield return new WaitForSeconds(0.1f); // Small delay to ensure activation

        // Unload the Briefing scene by name
        AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(briefingSceneName);

        // Optionally, wait until the unload operation is done
        while (!unloadOperation.isDone)
        {
            yield return null; // Wait until unload is complete
        }

        //Debug.Log("Briefing scene unloaded."); // Log confirmation
    }
}