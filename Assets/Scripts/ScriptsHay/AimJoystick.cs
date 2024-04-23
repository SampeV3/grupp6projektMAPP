using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AimJoystick : MonoBehaviour
{
    private Joystick joystick;
    public GameObject pistol_1, pistol_2; //ändrats av Basir
    Vector2 GameobjectRotation;
    private float GameobjectRotation2;
    private float GameobjectRotation3;
    private bool isInvokingShooting = false;
    

    public bool FacingRight = true;
    PlayerShoot playerShoot;
    UIController uIController;

    private void Awake()
    {
        playerShoot = GetComponent<PlayerShoot>();
        uIController = GameObject.FindGameObjectWithTag("UIController").gameObject.GetComponent<UIController>(); //tillagt av Basir
    }

    private void OnAim(InputValue inputValue)
    {
        GameobjectRotation = inputValue.Get<Vector2>();
    }

    private void FireInvocation ()
    {
        playerShoot.Shoot();
        isInvokingShooting = false;
    }

    void Update()
    {
        //Gets the input from the jostick

        bool isStill = GameobjectRotation == Vector2.zero;
        if (!isStill && !isInvokingShooting) {
            isInvokingShooting = true;
            Invoke("FireInvocation", 0.2f);
        }
                

    
        GameobjectRotation3 = GameobjectRotation.x;

        if (uIController.weapon_1_Selected) //tillagt av Basir
        {
            FlipWeapon(pistol_1);
            pistol_2.SetActive(false);
            pistol_1.SetActive(true);
        }

        if (uIController.weapon_2_Selected) //tillagt av Basir
        {
            FlipWeapon(pistol_2);
            pistol_1.SetActive(false);
            pistol_2.SetActive(true);
        }

    }
    private void FlipWeapon(GameObject pistol) //Raderna gjordes till en egen metod (av Basir)
    {
        if (FacingRight)
        {
            //Rotates the object if the player is facing right
            GameobjectRotation2 = GameobjectRotation.x + GameobjectRotation.y * 90;
            pistol.transform.rotation = Quaternion.Euler(0f, 0f, GameobjectRotation2);
        }
        else
        {
            //Rotates the object if the player is facing left
            GameobjectRotation2 = GameobjectRotation.x + GameobjectRotation.y * -90;
            pistol.transform.rotation = Quaternion.Euler(0f, 180f, -GameobjectRotation2);
        }
        if (GameobjectRotation3 < 0 && FacingRight)
        {
            // Executes the void: Flip()
            Flip();
        }
        else if (GameobjectRotation3 > 0 && !FacingRight)
        {
            // Executes the void: Flip()
            Flip();
        }
    }
    private void Flip()
    {
        // Flips the player.
        FacingRight = !FacingRight;

        transform.Rotate(0, 180, 0);
    }
}
