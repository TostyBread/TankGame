using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public GameObject MainMenuUI;
    public GameObject CreditsUI;

    private void Start()
    {
        if (MainMenuUI != null && CreditsUI != null)
        {
            MainMenuUI.SetActive(true);
            CreditsUI.SetActive(false);
        }
    }

    public void StartLevel1() // To Level 1
    {
        SceneManager.LoadScene("Level1");
    }

    public void StartBriefing() // To Briefing
    {
        SceneManager.LoadScene("Briefing");
    }

    public void CreditsButton() // To Credits during Main Menu
    {
        MainMenuUI.SetActive(false);
        CreditsUI.SetActive(true);
    }

    public void MainMenuButton() // To Main Menu from Credits
    {
        MainMenuUI.SetActive(true);
        CreditsUI.SetActive(false);
    }

    public void RestartGame() // Reloads current scene
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public void ReturnMainMenu() // To Main Menu
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame() // Exit Game
    {
        Application.Quit();
    }
}
