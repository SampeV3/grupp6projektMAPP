using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public Animator transition;

    public ParticleSystem playButtonEffekt;

    public GameObject settingsPanel, localizationsPanel;
    public List<GameObject> mainMenuButtons;

    public AudioClip clickSound, openPanelSound, closePanelSound;

    private AudioSource audioSource;

    private void Start()
    {
        settingsPanel.SetActive(false);
        localizationsPanel.SetActive(false);
        Time.timeScale = 1.0f;
        audioSource = GetComponent<AudioSource>();
        foreach (GameObject go in mainMenuButtons)
        {
            go.SetActive(true);
        }
    }

    public void StartGame()
    {
        StartCoroutine(AnimationDelay("Load", 1.0f));
        Time.timeScale = 1; //ändrar time scale till 1 igen om man har kommit hit från pause panelen i spelet
        audioSource.PlayOneShot(clickSound);
    }

    public void QuitGame()
    {
        StartCoroutine(AnimationDelay("Quit", 1.0f));
        SetMainMenuButtonsAnimations("Disabled", mainMenuButtons);
        audioSource.PlayOneShot(clickSound);
    }

    public void OpenLocalizationsPanel()
    {
        localizationsPanel.SetActive(true);
        SetMainMenuButtonsAnimations("Disabled", mainMenuButtons);
        playButtonEffekt.Stop();
        audioSource.PlayOneShot(openPanelSound);
    }

    public void ExitLocalizationsPanel()
    {
        StartCoroutine(AnimationDelay("ExitLocalization", 1f));
        SetMainMenuButtonsAnimations("Normal", mainMenuButtons);
        playButtonEffekt.Play();
        audioSource.PlayOneShot(openPanelSound);
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        SetMainMenuButtonsAnimations("Disabled", mainMenuButtons);
        playButtonEffekt.Stop();
        audioSource.PlayOneShot(openPanelSound);
    }
    public void ExitSettings()
    {
        StartCoroutine(AnimationDelay("ExitSettings", 1f));
        SetMainMenuButtonsAnimations("Normal", mainMenuButtons);
        playButtonEffekt.Play();
        audioSource.PlayOneShot(closePanelSound);
    }

    

    private IEnumerator AnimationDelay(string command, float delayTime)
    {
        if (command == "ExitSettings")
        {
            settingsPanel.GetComponent<Animator>().SetTrigger("Exit");
            yield return new WaitForSeconds(delayTime);
            settingsPanel.SetActive(false);
        }
        
        if (command == "Load")
        {
            yield return new WaitForSeconds(delayTime);
            transition.SetTrigger("Start");
            SceneManager.LoadScene(1);
        }

        if (command == "Quit")
        {
            yield return new WaitForSeconds(delayTime);
            transition.SetTrigger("Start");
            Application.Quit();
        }

        if (command == "ExitLocalization")
        {
            localizationsPanel.GetComponent<Animator>().SetTrigger("End");
            yield return new WaitForSeconds(delayTime);
            localizationsPanel.SetActive(false);
        }

        
        
    }

    private void SetMainMenuButtonsAnimations(string trigger, List<GameObject> buttons)
    {
        foreach (GameObject go in buttons)
        {
            go.GetComponent<Animator>().SetTrigger(trigger);
        }
    }
}
