﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MinibossAI : MonoBehaviour
{
    //[SerializeField] private Material flashOnHit;
    [SerializeField] private GameObject projectilePrefab, beam1, beam2, mortar;
    [SerializeField] private Transform player;
    //[SerializeField] private AudioClip projectileSFX;
    [SerializeField] private Animator anim;

    private AudioSource audioSource;
    private Material originalMat;
    private int HP;
    private int beamDirection;
    private bool beamsActive = false;
    private Coroutine combatCoroutine;
    void Start()
    {
        beamDirection = 1;
        HP = 100;
        audioSource = GetComponent<AudioSource>();
        combatCoroutine = StartCoroutine(Combat());
    }

    private IEnumerator Combat()
    {
        StartCoroutine(FlashBeam(beam1, 4));
        yield return new WaitUntil(() => beamsActive);
        while (HP > 75)
        {

            RangedAttack(0);
            yield return new WaitForSeconds(0.3f);
            anim.SetTrigger("Shoot");
            yield return new WaitForSeconds(1f);
            
            RangedAttack(22.5f);
            yield return new WaitForSeconds(0.3f);
            anim.SetTrigger("Shoot");
            yield return new WaitForSeconds(1f);

            HP = HP - 50;
        }
        StartCoroutine(FlashBeam(beam2, 4));
        while (HP > 50)
        {
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < Random.Range(3, 7); j++)
                {

                    RangedAttack(0);
                    yield return new WaitForSeconds(0.3f);
                    anim.SetTrigger("Shoot");
                    yield return new WaitForSeconds(1f);

                    RangedAttack(22.5f);
                    yield return new WaitForSeconds(0.3f);
                    anim.SetTrigger("Shoot");
                    yield return new WaitForSeconds(1f);
                }
                StartCoroutine(RotateBeams());
            }
        }
        while (HP > 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < Random.Range(3, 7); j++)
                    {

                        RangedAttack(0);
                        yield return new WaitForSeconds(0.3f);
                        anim.SetTrigger("Shoot");
                        yield return new WaitForSeconds(1f);

                        StartCoroutine(SpawnMortar());

                        RangedAttack(22.5f);
                        yield return new WaitForSeconds(0.3f);
                        anim.SetTrigger("Shoot");
                        yield return new WaitForSeconds(1f);
                    }
                    StartCoroutine(RotateBeams());
                }
            }
        
    }
    private IEnumerator FlashBeam(GameObject beam, int flashAmount)
    {
        SpriteRenderer beamsprd = beam.GetComponent<SpriteRenderer>();
        beam.SetActive(true);
        beamsprd.color = new Color(beamsprd.color.r, beamsprd.color.g, beamsprd.color.b, 0f);
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < flashAmount; i++)
        {
            beamsprd.color = new Color(beamsprd.color.r, beamsprd.color.g, beamsprd.color.b, 0.1f);
            yield return new WaitForSeconds(0.5f);
            beamsprd.color = new Color(beamsprd.color.r, beamsprd.color.g, beamsprd.color.b, 0f);
            yield return new WaitForSeconds(0.4f);
        }   
        beamsprd.color = new Color(beamsprd.color.r, beamsprd.color.g, beamsprd.color.b, 1f);
        beamsActive = true;
    }

    private IEnumerator SpawnMortar()
    {
        GameObject mortarAim = Instantiate(mortar, player.position, Quaternion.identity);
        Transform desiredChild = mortarAim.transform.Find("MortarBackground");
        SpriteRenderer mortarsprd = desiredChild.GetComponent<SpriteRenderer>();
        Color ogColor = mortarsprd.color;
        ogColor.a = 0.1f;
        mortarsprd.color = ogColor;
        while (mortarsprd.color.a < 1f)
        {
            mortarAim.transform.Rotate(0f, 0f, 15f * Time.fixedDeltaTime);
            ogColor.a += 0.5f* Time.deltaTime;
            mortarsprd.color = ogColor;
            yield return null;
        }
        Destroy(mortarAim);
    }
    private IEnumerator RotateBeams()
    {
        SpriteRenderer beamsprd1 = beam1.GetComponent<SpriteRenderer>();
        SpriteRenderer beamsprd2 = beam2.GetComponent<SpriteRenderer>();
        Color ogColor = beamsprd1.color;
        for (int i = 0; i < 3; i++)
        {
            beamsprd1.color = ogColor;
            beamsprd2.color = ogColor;
            yield return new WaitForSeconds(0.4f);
            beamsprd1.color = Color.red;
            beamsprd2.color = Color.red;
            yield return new WaitForSeconds(0.4f);

        }
        beamDirection = beamDirection * -1;
        beamsprd1.color = ogColor;
        beamsprd2.color = ogColor;
    }
    private void RangedAttack(float offset)
    {
        for (int i = 0; i < 8; i++)
        {
            Vector2 direction = new Vector2(Mathf.Cos((offset + i * 45f) * Mathf.Deg2Rad), Mathf.Sin((offset + i * 45f) * Mathf.Deg2Rad));
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.Euler(0f, 0f, offset + i * 45f));
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            projectile.GetComponent<Rigidbody2D>().rotation = angle;
            projectile.GetComponent<Rigidbody2D>().velocity = direction * 0.8f;    
        }
    }

    void FixedUpdate()
    {
        if (beamsActive)
        {
            transform.Rotate(0f, 0f, beamDirection*8f * Time.fixedDeltaTime);
        }
    }
}