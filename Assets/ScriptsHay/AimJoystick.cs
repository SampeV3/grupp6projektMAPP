using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AimJoystick : MonoBehaviour
{
    private Joystick joystick;
    public GameObject Object;
    Vector2 GameobjectRotation;
    private float GameobjectRotation2;
    private float GameobjectRotation3;
    private bool isInvokingShooting = false;
    

    public bool FacingRight = true;
    PlayerShoot playerShoot;

    private void Awake()
    {
        playerShoot = GetComponent<PlayerShoot>();
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

        if (FacingRight)
        {
            //Rotates the object if the player is facing right
            GameobjectRotation2 = GameobjectRotation.x + GameobjectRotation.y * 90;
            Object.transform.rotation = Quaternion.Euler(0f, 0f, GameobjectRotation2);
        }
        else
        {
            //Rotates the object if the player is facing left
            GameobjectRotation2 = GameobjectRotation.x + GameobjectRotation.y * -90;
            Object.transform.rotation = Quaternion.Euler(0f, 180f, -GameobjectRotation2);
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
