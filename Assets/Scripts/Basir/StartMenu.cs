using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public Animator transition;
    public GameObject settingsPanel, mainMenu;

    public AudioClip clickSound, openPanelSound, closePanelSound;

    private AudioSource audioSource;
    //public GameObject transitionImage;
    //private bool transitionPlayed;

    private void Start()
    {
        Time.timeScale = 1.0f;
        audioSource = GetComponent<AudioSource>();
    }

    public void StartGame()
    {
        StartCoroutine(AnimationDelay("Load", 1.0f));
        Time.timeScale = 1;
        audioSource.PlayOneShot(clickSound);
    }

    public void QuitGame()
    {
        StartCoroutine(AnimationDelay("Quit", 1.0f));
        audioSource.PlayOneShot(clickSound);
    }

    public void OpenSettings()
    {
        audioSource.PlayOneShot(openPanelSound);
    }
    public void ExitSettings()
    {
        audioSource.PlayOneShot(closePanelSound);
    }

    

    private IEnumerator AnimationDelay(string command, float delayTime)
    {
        //transitionImage.SetActive(true);
        

        yield return new WaitForSeconds(delayTime);
        
        if (command == "Load")
        {
            transition.SetTrigger("Start");
            SceneManager.LoadScene(1);
        }

        if (command == "Quit")
        {
            transition.SetTrigger("Start");
            Application.Quit();
        }
        
    }
}
