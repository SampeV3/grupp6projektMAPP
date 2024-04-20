using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public void StartGame()
    {
        Time.timeScale = 0.5f;
        SceneManager.LoadScene(1);

    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
