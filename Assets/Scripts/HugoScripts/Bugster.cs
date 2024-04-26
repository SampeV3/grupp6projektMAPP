using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Bugster : MonoBehaviour
{

    [SerializeField] private GameObject player, playerSpotted;

    [SerializeField] private Transform castUpX, castDownX, castRightY, castLeftY;

    [SerializeField] private LayerMask whatIsWall, playerMask;

    private float totalVelocity = 2.0f;
    private float totalDistance;
    private float velocityX = 0;
    private float velocityY = 0;
    private float distanceX;
    private float distanceY;
    private float proportion;

    private bool playerDetected = false;
    private bool isCollided = false;

    private int direction = 1; //-1 = facing right, 1 == facing left

    private RaycastHit2D xHit, yHit;
    private Coroutine moveCoroutine, combatCoroutine;
    private GameObject playerSpottedWarning;


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
            CheckWall();
            if(isCollided == false) {
                transform.Translate(new Vector2(velocityX*direction, velocityY) * Time.deltaTime);
            }
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

    void FixedUpdate() {
        if (!playerDetected)
        {
            if (IsPlayerWithinDetectionRadius())
            {
                CheckLineOfSight();
                
            }
        }
        else
        {
            if (!IsPlayerWithinDetectionRadius())
            {
                playerDetected = false;
                if (combatCoroutine != null)
                {
                    StopCoroutine(moveCoroutine);
                    StopCoroutine(combatCoroutine);
                }
            }
        }
    }

    private void CheckWall() {
        if(direction == 1) {
            if(velocityY > 0) {
                xHit = Physics2D.Raycast(castDownX.position, Vector2.left, 0.5f, whatIsWall);
                //Debug.DrawRay(castDownX.position, Vector2.left * 0.5f, Color.red, 0.5f);
                yHit = Physics2D.Raycast(castRightY.position, Vector2.up, 0.25f, whatIsWall);
                //Debug.DrawRay(castRightY.position, Vector2.up * 0.25f, Color.red, 0.25f);
            }
            else if(velocityY < 0) {
                xHit = Physics2D.Raycast(castUpX.position, Vector2.left, 0.5f, whatIsWall);
                //Debug.DrawRay(castUpX.position, Vector2.left * 0.5f, Color.red, 0.5f);
                yHit = Physics2D.Raycast(castRightY.position, Vector2.down, 0.25f, whatIsWall);
                //Debug.DrawRay(castRightY.position, Vector2.down * 0.25f, Color.red, 0.25f);
            }
        }
        else if(direction == -1) {
            if(velocityY > 0) {
                xHit = Physics2D.Raycast(castDownX.position, Vector2.right, 0.5f, whatIsWall);
                //Debug.DrawRay(castDownX.position, Vector2.right * 0.5f, Color.red, 0.5f);
                yHit = Physics2D.Raycast(castRightY.position, Vector2.up, 0.25f, whatIsWall);
                //Debug.DrawRay(castRightY.position, Vector2.up * 0.25f, Color.red, 0.25f);
            }
            else if(velocityY < 0) {
                xHit = Physics2D.Raycast(castUpX.position, Vector2.right, 0.5f, whatIsWall);
                //Debug.DrawRay(castUpX.position, Vector2.right * 0.5f, Color.red, 0.5f);
                yHit = Physics2D.Raycast(castRightY.position, Vector2.down, 0.25f, whatIsWall);
                //Debug.DrawRay(castRightY.position, Vector2.down * 0.25f, Color.red, 0.25f);
            }
        }
        

        if(xHit.collider != null && xHit.collider.CompareTag("Wall")) {
            isCollided = true;
            if(velocityY > 0) {
                transform.Translate(new Vector2(0, totalVelocity) * Time.deltaTime);
            }
            else if(velocityY < 0) {
                transform.Translate(new Vector2(0, -totalVelocity) * Time.deltaTime);
            }
            
        }
        else if(yHit.collider != null && yHit.collider.CompareTag("Wall")) {
            isCollided = true;
            transform.Translate(new Vector2(-totalVelocity, 0) * Time.deltaTime);
        }
        else {
            isCollided = false;
        }
        
    }

    private bool IsPlayerWithinDetectionRadius()
    {
   
        float dist = Vector2.Distance(transform.position, player.transform.position);
        if (dist <= 8f)
        {   
            return true;
        }
        return false;
    }

    private void CheckLineOfSight()
    {
        Vector3 directionToPlayer = player.transform.position - transform.position;

        RaycastHit2D hitPlayer = Physics2D.Raycast(transform.position, directionToPlayer, 50f, playerMask);
        RaycastHit2D hitWall = Physics2D.Raycast(transform.position, directionToPlayer, 50f, whatIsWall);

        if (hitWall.collider.CompareTag("Wall") && hitPlayer.collider.CompareTag("Player") && hitPlayer.distance<hitWall.distance)
        {
            playerDetected = true;
                GameObject playerSpottedWarning = Instantiate(playerSpotted, transform.position, Quaternion.identity, transform);
                playerSpottedWarning.name = "PlayerSpotted";
                playerSpottedWarning.GetComponent<Animator>().SetTrigger("PlayerDetected");
                Destroy(playerSpottedWarning, 1f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("PlayerAttack")) {
            Destroy(gameObject);
        }
    }
    
}
