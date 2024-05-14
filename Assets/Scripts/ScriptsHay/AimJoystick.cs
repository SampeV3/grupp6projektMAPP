using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class AimJoystick : MonoBehaviour
{
    private Joystick joystick;
    public GameObject pistol_1, pistol_2; //�ndrats av Basir
    Vector2 _aimInput = Vector2.zero;
    private float GameobjectRotation2;
    private float GameobjectRotation3;
    private bool isInvokingShooting = false;

    public bool use360Rotation = true;
    
    public bool FacingRight = true;
    PlayerShoot playerShoot;
    Laser Laser;
    public GameObject PlasmaGun;
    UIController uIController;
    
    private Vector2 _movemenetInputSmoothVelocity = Vector2.zero;
    private Vector2 _smoothedMovementInput;

    private void Awake()
    {
        playerShoot = GetComponent<PlayerShoot>();
        Laser = PlasmaGun.GetComponent<Laser>(); 
        uIController = GameObject.FindGameObjectWithTag("UIController").gameObject.GetComponent<UIController>(); //tillagt av Basir
    }

    public void OnAim(Vector2 direction)
    {
        _aimInput = direction;
        print(_aimInput);
    }

    private void FireInvocation ()
    {
        playerShoot.Shoot();
        isInvokingShooting = false;
    }

    void Update()
    {
        //Gets the input from the jostick

        bool isStill = _aimInput == Vector2.zero;
        if (!isStill && !isInvokingShooting) {

            if(uIController.weapon_1_Selected)
            {
                isInvokingShooting = true;
                Invoke("FireInvocation", 0.2f);
            }

            if(uIController.weapon_2_Selected)
            {
                Laser.EnableLaser();
            }
            
        }
        else { Laser.DisableLaser(); }
        
        
        

        GameObject activeWeapon = null;
        
        if (uIController.weapon_1_Selected) //tillagt av Basir
        {
            activeWeapon = pistol_1;
            pistol_2.SetActive(false);
            pistol_1.SetActive(true);
        }

        
     
        if (uIController.weapon_2_Selected)
         {
               // Instantiate(bulletPrefab, firePoint2.position, firePoint2.rotation);
               activeWeapon = pistol_2;
            pistol_1.SetActive(false);
            pistol_2.SetActive(true);
         }
        
        if (use360Rotation)
        {
            
            _smoothedMovementInput = Vector2.SmoothDamp(
                _smoothedMovementInput,
                _aimInput,
                ref _movemenetInputSmoothVelocity,
                0.01f
            );
            RotateInDirectionOfInput(activeWeapon);
            //Vector3 relativePos = _aimInput.position - transform.position;
            //Quaternion targetRotation = Quaternion.LookRotation(relativePos);
            //activeWeapon.transform.rotation = Quaternion.RotateTowards( activeWeapon.transform.rotation, , Time.deltaTime);

        }
        else
        {
            FlipWeapon(activeWeapon);
        }
           
        
    }

    private float rotationSpeed = 100f;
    private void RotateInDirectionOfInput(GameObject weapon)
    {
        if (_aimInput != Vector2.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(weapon.transform.forward, _smoothedMovementInput);
            Quaternion rotation = Quaternion.RotateTowards(weapon.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            weapon.transform.rotation = rotation;
            //Quaternion offset = Quaternion.Euler(0, 0, -90); // Rotate by -90 degrees around Z-axis
            //weapon.transform.rotation *= offset; // Apply the rotation to your object
            //_rigidbody.MoveRotation(rotation);
        } 
    }

    private void FlipWeapon(GameObject pistol) //Raderna gjordes till en egen metod (av Basir)
    {
        
        if (FacingRight)
        {
            //Rotates the object if the player is facing right
            GameobjectRotation2 = _aimInput.x + _aimInput.y * 90;
            pistol.transform.rotation = Quaternion.Euler(0f, 0f, GameobjectRotation2);
        }
        else
        {
            //Rotates the object if the player is facing left
            GameobjectRotation2 = _aimInput.x + _aimInput.y * -90;
            pistol.transform.rotation = Quaternion.Euler(0f, 180f, -GameobjectRotation2);
        }
        GameobjectRotation3 = _aimInput.x;
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
