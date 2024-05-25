using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public Animator transition;

    public ParticleSystem playButtonEffekt;

    public GameObject settingsPanel, localizationsPanel, quitButtonPopUpPanel, loadingPanel;




    private void Start()
    {
        settingsPanel.SetActive(false);
        localizationsPanel.SetActive(false);
        quitButtonPopUpPanel.SetActive(false);
        loadingPanel.SetActive(false);
        Time.timeScale = 1.0f; //ändrar time scale till 1 igen om man har kommit hit från pause panelen i spelet

    }

    public void StartGame()
    {
        loadingPanel.SetActive(true);
        playButtonEffekt.Stop();
        StartCoroutine(AnimationDelay("Load", 4f));
    }

    public void OpenQuitButtonPopUpPanel()
    {
        quitButtonPopUpPanel.SetActive(true);
        playButtonEffekt.Stop();
    }

    public void ExitQuitButtonPopUpPanel()
    {
        playButtonEffekt.Play();
        StartCoroutine(AnimationDelay("QuitButtonPopUpPanel", 0.5f));
    }

    public void QuitGame()
    {
        StartCoroutine(AnimationDelay("Quit", 1.0f));
        
    }

    public void OpenLocalizationsPanel()
    {
        localizationsPanel.SetActive(true);
        
        playButtonEffekt.Stop();
    }

    public void ExitLocalizationsPanel()
    {
        StartCoroutine(AnimationDelay("ExitLocalization", 1f));
        
        playButtonEffekt.Play();
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        
        playButtonEffekt.Stop();
        
    }
    public void ExitSettings()
    {
        StartCoroutine(AnimationDelay("ExitSettings", 1f));
        playButtonEffekt.Play();
        
    }

    

    private IEnumerator AnimationDelay(string command, float delayTime)
    {
        if (command == "ExitSettings")
        {
            settingsPanel.GetComponent<Animator>().SetTrigger("End");
            yield return new WaitForSecondsRealtime(delayTime);
            settingsPanel.SetActive(false);
        }
        
        if (command == "Load") //Loading Panel Animation
        {
            yield return new WaitForSecondsRealtime(delayTime/2);
            transition.SetTrigger("Start");
            yield return new WaitForSecondsRealtime(delayTime/5);
            SceneManager.LoadScene(1);
        }

        if (command == "Quit")
        {
            transition.SetTrigger("Start");
            yield return new WaitForSecondsRealtime(delayTime);
            Application.Quit();
        }

        if (command == "ExitLocalization")
        {
            localizationsPanel.GetComponent<Animator>().SetTrigger("End");
            yield return new WaitForSecondsRealtime(delayTime);
            localizationsPanel.SetActive(false);
        }

        if (command == "QuitButtonPopUpPanel")
        {
            quitButtonPopUpPanel.GetComponent<Animator>().SetTrigger("End");
            yield return new WaitForSecondsRealtime(delayTime);
            quitButtonPopUpPanel.SetActive(false);
        }

        
        
    }
}
