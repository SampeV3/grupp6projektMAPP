using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    public Animator crossfadeTransition;

    public ParticleSystem playButtonEffekt;

    public Button playButton;

    public List<GameObject> panelsInactiveAtStart;

    public GameObject loadingPanel;

    private float crossfadeTransitionLength;




    private void Start()
    {
        foreach (GameObject go in panelsInactiveAtStart) 
        {
            go.SetActive(false); 
        }

        Time.timeScale = 1.0f; //ändrar time scale till 1 igen om man har kommit hit från pause panelen i spelet

        crossfadeTransitionLength = crossfadeTransition.GetCurrentAnimatorStateInfo(0).length;

    }

    private void Update()
    {
        if (IsAnyPanelActive())
        {
            playButton.GetComponent<Animator>().enabled = false;
            playButtonEffekt.Stop();
        }
        else
        {
            playButton.GetComponent <Animator>().enabled = true;
            playButtonEffekt.Play();
        }
    }

    private bool IsAnyPanelActive()
    {
        foreach(GameObject go in panelsInactiveAtStart)
        {
            if (go.activeSelf)
            {
                return true;
            }
        }
        return false;
    }

    public void OpenAnyPanel( GameObject gameObject ) 
    {
        Animator gameObjectAnimator = gameObject.GetComponent<Animator>();
        float delay = gameObjectAnimator.GetCurrentAnimatorStateInfo(0).length + 0.01f;
        StartCoroutine(AnimationDelay("OpenAnyPanel", delay, gameObject, gameObjectAnimator));
    }

    public void ExitAnyPanel( GameObject gameObject )
    {
        Animator gameObjectAnimator = gameObject.GetComponent<Animator>();
        float delay = gameObjectAnimator.GetCurrentAnimatorStateInfo(0).length + 0.01f;
        StartCoroutine(AnimationDelay("ExitAnyPanel", delay , gameObject, gameObjectAnimator));

    }

    public void QuitGame()
    {
        StartCoroutine(AnimationDelay("QuitGame", crossfadeTransitionLength, null, crossfadeTransition));
    }

    

    private IEnumerator AnimationDelay(string command, float delay, GameObject gameObject, Animator gameObjectAnimator)
    {
        if (command == "OpenAnyPanel" && gameObject == loadingPanel)
        {
            gameObject.SetActive(true);
            yield return new WaitForSecondsRealtime(delay - crossfadeTransitionLength);
            crossfadeTransition.SetTrigger("Start");
            yield return new WaitForSecondsRealtime(crossfadeTransitionLength);
            SceneManager.LoadScene(1);
        }

        if (command == "OpenAnyPanel" && gameObject != loadingPanel)
        {
            gameObject.SetActive(true);
            yield break;
        }

        if (command == "ExitAnyPanel")
        {
            if (!gameObject.activeSelf)
            {
                yield break;
            }
            else
            {
                gameObjectAnimator.SetTrigger("End");
                yield return new WaitForSecondsRealtime(delay);
                gameObject.SetActive(false);
            }

        }

        if (command == "QuitGame")
        {
            gameObjectAnimator.SetTrigger("Start");
            yield return new WaitForSecondsRealtime(delay);
            Application.Quit();
        }
    }
}
