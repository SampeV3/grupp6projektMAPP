using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public Animator transition;
    //public GameObject transitionImage;
    //private bool transitionPlayed;

    private void Start()
    {
        Time.timeScale = 1.0f;
    }

    public void StartGame()
    {
        StartCoroutine(AnimationDelay("Load"));
        Time.timeScale = 1;
        
    }

    public void QuitGame()
    {
        StartCoroutine(AnimationDelay("Quit"));
    }

    private IEnumerator AnimationDelay(string command)
    {
        //transitionImage.SetActive(true);
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(1);
        
        if (command == "Load")
        {
            SceneManager.LoadScene(1);
        }

        if (command == "Quit")
        {
            Application.Quit();
        }
        
    }
}
