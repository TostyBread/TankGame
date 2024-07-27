using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void StartLevel1() // To Level 1
    {
        SceneManager.LoadScene("Level1");
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
