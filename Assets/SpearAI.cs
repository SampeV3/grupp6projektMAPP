﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Lumin;
using Codice.CM.Client.Differences;

public class SpearAI : MonoBehaviour
{
    [SerializeField] private Material flashOnHit;
    [SerializeField] private GameObject playerSpotted;
    //[SerializeField] private AudioClip projectileSFX;
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask playerMask, obstacleMask;

    private AudioSource audioSource;
    private Material originalMat;
    private SpriteRenderer sprd;
    private Animator anim;
    private int HP;
    private bool playerDetected = false;
    private bool canhitSomething = false;
    private bool isDead, droppedLoot = false;
    private Coroutine combatCoroutine, rotateCoroutine;
    private GameObject playerSpottedWarning;
    private bool hitSomething = false;
    private float rotateFactor;

    void Start()
    {
        HP = 10;
        audioSource = GetComponent<AudioSource>();
        //anim = GetComponent<Animator>();
        sprd = GetComponent<SpriteRenderer>();
        originalMat = sprd.material;
        anim = GetComponent<Animator>();



    }

    private void Awake()
    {
        if (player == null)
        {
            player = IsPlayer.FindPlayerTransformAutomaticallyIfNull(); //Tillagt av Elias
        }
    }


    private void Update()
    {
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
                    StopCoroutine(combatCoroutine);
                }
            }
        }
    }

    private bool IsPlayerWithinDetectionRadius()
    {
        float dist = Vector2.Distance(transform.position, player.position);
        if (dist <= 20f)
        {
            return true;
        }
        return false;
    }


    private IEnumerator Combat()
    {
        while (HP > 0)
        {
            rotateCoroutine = StartCoroutine(RotateTowardsPlayer());
            anim.SetTrigger("Charge");
            rotateFactor = 100f;
            yield return new WaitForSeconds(2.5f);
            
            anim.SetTrigger("Dash");
            Invoke("CooldownReset", 0.03f);
            hitSomething = false;
            rotateFactor = 45f;
            while (!hitSomething) // Continue moving until hitSomething is true
            {
                transform.position = transform.position + -transform.up * 10f * Time.deltaTime;
                yield return null;
            }
            StopCoroutine(rotateCoroutine);
            anim.SetTrigger("Stunned");
            canhitSomething = false;

            yield return new WaitForSeconds(2f);
        }
    }
    private void CooldownReset()
    {
        canhitSomething = true;
    }

    private IEnumerator RotateTowardsPlayer()
    {
        while (true)
        {
            Vector2 directionToPlayer = player.position - transform.position;
            float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
            Quaternion desiredRotation = Quaternion.Euler(0f,0f, angle+90f);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, rotateFactor * Time.deltaTime);
            yield return null;
        }
    }

    private void CheckLineOfSight()
    {
        Vector3 directionToPlayer = player.position - transform.position;

        RaycastHit2D hitPlayer = Physics2D.Raycast(transform.position, directionToPlayer, 50f, playerMask);
        RaycastHit2D hitWall = Physics2D.Raycast(transform.position, directionToPlayer, 50f, obstacleMask);

        if (hitWall.collider.CompareTag("Wall") && hitPlayer.collider.CompareTag("Player") && hitPlayer.distance < hitWall.distance)
        {
            playerDetected = true;
            GameObject playerSpottedWarning = Instantiate(playerSpotted, transform.position, Quaternion.identity, transform);
            playerSpottedWarning.name = "PlayerSpotted";
            playerSpottedWarning.GetComponent<Animator>().SetTrigger("PlayerDetected");
            Destroy(playerSpottedWarning, 1f);
            combatCoroutine = StartCoroutine(Combat());
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PlayerAttack"))
        {
            Destroy(other.gameObject);
            HP -= 1;
            StartCoroutine(Flash());
            if (HP <= 0)
            {
                isDead = true;
                if (combatCoroutine != null)
                {
                    StopCoroutine(combatCoroutine);
                }
                sprd.color = Color.red;
                dropLoot();
                Destroy(gameObject, 1f);
            }
        }
        if (canhitSomething)
        {
            if (other.gameObject.CompareTag("Wall"))
            {
                hitSomething = true;
            }
            if (other.gameObject.CompareTag("Player"))
            {
                hitSomething = true;
            }
        }

    }

    private void dropLoot()
    {
        if (!droppedLoot)
        {
            droppedLoot = true;
            GetComponent<LootBag>().InstantiateLoot(transform.position);

        }

    }

    private IEnumerator Flash()
    {
        if (!isDead)
        {
            sprd.material = flashOnHit;
            yield return new WaitForSeconds(0.125f);
            sprd.material = originalMat;

        }
    }
}
