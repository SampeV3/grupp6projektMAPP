using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Bugster : MonoBehaviour
{

    [SerializeField] private GameObject player;

    [SerializeField] private Transform castUpX, castDownX, castRightY, castLeftY;

    [SerializeField] private LayerMask whatIsWall;

    private float totalVelocity = 2.0f;
    private float totalDistance;
    private float velocityX = 0;
    private float velocityY = 0;
    private float distanceX;
    private float distanceY;
    private float proportion;

    private bool playerDetected = false;

    private int direction = 1; //-1 = facing right, 1 == facing left

    RaycastHit2D xHit, yHit;


    // Update is called once per frame
    void Update()
    {
        if(playerDetected == true) {
            distanceX = player.transform.position.x-transform.position.x;
            distanceY = player.transform.position.y-transform.position.y;
            totalDistance = MathF.Sqrt((distanceY*distanceY)+(distanceX*distanceX));
            proportion = totalDistance/totalVelocity;
            velocityX = distanceX/proportion;
            velocityY = distanceY/proportion;
            transform.Translate(new Vector2(velocityX*direction, velocityY) * Time.deltaTime);
            CheckWall();
            if(velocityX > 0 && direction == 1) {
                transform.rotation = Quaternion.Euler(new Vector3(0, 1, 0) * 180);
                direction = -1;
            }
            else if(velocityX < 0 && direction == -1) {
                transform.rotation = Quaternion.Euler(Vector3.zero);
                direction = 1;
            }
        }   
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("Player")) {
            playerDetected = true;
            
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.gameObject.CompareTag("Player")) {
            playerDetected = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.CompareTag("PlayerAttack")) {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }

    private void CheckWall() {
        if(direction == 1) {
            if(velocityY > 0) {
                xHit = Physics2D.Raycast(castDownX.position, Vector2.left, 0.5f, whatIsWall);
                //Debug.DrawRay(castDownX.position, Vector2.left * 0.5f, Color.red, 0.5f);
            }
            else if(velocityY < 0) {
                xHit = Physics2D.Raycast(castUpX.position, Vector2.left, 0.5f, whatIsWall);
                //Debug.DrawRay(castUpX.position, Vector2.left * 0.5f, Color.red, 0.5f);
            }
        }
        else if(direction == -1) {
            if(velocityY > 0) {
                xHit = Physics2D.Raycast(castDownX.position, Vector2.right, 0.5f, whatIsWall);
                //Debug.DrawRay(castDownX.position, Vector2.right * 0.5f, Color.red, 0.5f);
            }
            else if(velocityY < 0) {
                xHit = Physics2D.Raycast(castUpX.position, Vector2.right, 0.5f, whatIsWall);
                //Debug.DrawRay(castUpX.position, Vector2.right * 0.5f, Color.red, 0.5f);
            }
        }

        if(velocityY > 0) {
            if(direction == 1) {
                yHit = Physics2D.Raycast(castRightY.position, Vector2.up, 0.25f, whatIsWall);
                //Debug.DrawRay(castRightY.position, Vector2.up * 0.25f, Color.red, 0.25f);
            }
            else if(direction == -1) {
                yHit = Physics2D.Raycast(castLeftY.position, Vector2.up, 0.25f, whatIsWall);
                //Debug.DrawRay(castLeftY.position, Vector2.up * 0.25f, Color.red, 0.25f);
            }
        }
        else if(velocityY < 0) {
            if(direction == 1) {
                yHit = Physics2D.Raycast(castRightY.position, Vector2.down, 0.25f, whatIsWall);
                //Debug.DrawRay(castRightY.position, Vector2.down * 0.25f, Color.red, 0.25f);
            }
            else if(direction == -1) {
                yHit = Physics2D.Raycast(castLeftY.position, Vector2.down, 0.25f, whatIsWall);
                //Debug.DrawRay(castLeftY.position, Vector2.down * 0.25f, Color.red, 0.25f);
            }
        }
        

        if(xHit.collider != null && xHit.collider.CompareTag("Wall")) {
            if(velocityY > 0) {
                transform.Translate(new Vector2(0, totalVelocity-1.0f) * Time.deltaTime);
            }
            else if(velocityY < 0) {
                transform.Translate(new Vector2(0, -totalVelocity+1.0f) * Time.deltaTime);
            }
            
        }
        if(yHit.collider != null && yHit.collider.CompareTag("Wall")) {
            if(direction == 1) {
                transform.Translate(new Vector2(-totalVelocity+1.0f, 0) * Time.deltaTime);
            }
            else if(direction == -1) {
                transform.Translate(new Vector2(totalVelocity-1.0f, 0) * Time.deltaTime);
            }
        }
    }
    
}
