using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeDetection : MonoBehaviour
{
    private CharacterMovement characterMovement;

    public void Initialize(CharacterMovement characterMovement)
    {
        this.characterMovement = characterMovement;
    }



    private void Update()
    {
        
         Vector3 shakeDetection = Input.acceleration;

        //Kollar efter om mobilen skakar eller ej. Om den skakar så ska den 
        // runna metoden Dash  från character movement scriptet
        if (shakeDetection.sqrMagnitude >= 5f)
        {
            if (characterMovement != null)
            {
                characterMovement.Dash();
            }
            else
            {
                Debug.LogError("CharacterMovement reference is null.");
            }
            
        }

    }
}
