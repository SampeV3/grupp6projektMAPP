using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public Animator transition;

    public ParticleSystem playButtonEffekt;

    public GameObject settingsPanel, localizationsPanel, quitButtonPopUpPanel;

    public AudioClip clickSound, openPanelSound, closePanelSound;

    private AudioSource audioSource;



    private void Start()
    {
        settingsPanel.SetActive(false);
        localizationsPanel.SetActive(false);
        quitButtonPopUpPanel.SetActive(false);
        Time.timeScale = 1.0f;
        audioSource = GetComponent<AudioSource>();
    }

    public void StartGame()
    {
        StartCoroutine(AnimationDelay("Load", 1.0f));
        Time.timeScale = 1; //ändrar time scale till 1 igen om man har kommit hit från pause panelen i spelet
        audioSource.PlayOneShot(clickSound);
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
        audioSource.PlayOneShot(clickSound);
    }

    public void OpenLocalizationsPanel()
    {
        localizationsPanel.SetActive(true);
        
        playButtonEffekt.Stop();
        audioSource.PlayOneShot(openPanelSound);
    }

    public void ExitLocalizationsPanel()
    {
        StartCoroutine(AnimationDelay("ExitLocalization", 1f));
        
        playButtonEffekt.Play();
        audioSource.PlayOneShot(openPanelSound);
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        
        playButtonEffekt.Stop();
        audioSource.PlayOneShot(openPanelSound);
    }
    public void ExitSettings()
    {
        StartCoroutine(AnimationDelay("ExitSettings", 1f));
        playButtonEffekt.Play();
        audioSource.PlayOneShot(closePanelSound);
    }

    

    private IEnumerator AnimationDelay(string command, float delayTime)
    {
        if (command == "ExitSettings")
        {
            settingsPanel.GetComponent<Animator>().SetTrigger("End");
            yield return new WaitForSecondsRealtime(delayTime);
            settingsPanel.SetActive(false);
        }
        
        if (command == "Load")
        {
            yield return new WaitForSecondsRealtime(delayTime);
            transition.SetTrigger("Start");
            SceneManager.LoadScene(1);
        }

        if (command == "Quit")
        {
            yield return new WaitForSecondsRealtime(delayTime);
            transition.SetTrigger("Start");
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
