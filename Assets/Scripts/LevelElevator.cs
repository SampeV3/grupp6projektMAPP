using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class LevelElevator : PromptQuestion
{
    private bool canOpenPrompt = true;
    public UIController uiController;
    public delegate void GoToNextLevel();

    public static event GoToNextLevel BeforeNextLevel;
    public static event GoToNextLevel ToNextLevel;    
    private void Awake()
    {
        if (this.name.Equals(""))
        {
            this.questionText = "Go up?";
            this.progressText = "Go up";
            this.cancelText = "Stay";
            this.name = "NextLevel";
        }
    }

    public override IEnumerator OnCancel(PromptData promptFields)
    {
        yield return new WaitForSeconds(0.1f);
        promptFields.promptObject.SetActive(false);
        yield return new WaitForSeconds(0.2f); //debounce
        canOpenPrompt = true;
    }

    public override IEnumerator OnProgress(PromptData promptFields)
    {
        
        print("YAY!");
        if (BeforeNextLevel != null) BeforeNextLevel();
        yield return new WaitForSeconds(1f);
        if (ToNextLevel != null) ToNextLevel();
    }

    private bool doorOpenedAnimationHasPlayed = false;
    private IEnumerator Open()
    {
        
        if (!doorOpenedAnimationHasPlayed)
        {
            GetComponent<Animator>().SetBool("Open", true);
            yield return new WaitForSeconds(2);
        }
        else
        {
            yield return new WaitForSeconds(0.1f); 
        }
        uiController.PromptQuestion(this);
        
    }
    
    private void OnCollisionEnter2D (Collision2D other)
    {
        print(other.gameObject.tag);
        print(other.gameObject.tag.Equals("LevelElevator") && canOpenPrompt);
        if (other.gameObject.tag.Equals("LevelElevator") && canOpenPrompt)
        {
            canOpenPrompt = false;
            StartCoroutine(Open());
        }
    }
    
    
}
