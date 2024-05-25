﻿
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MinibossAI : EnemyMonoBehaviour
{
    [SerializeField] private Material flashOnHit;
    [SerializeField] private GameObject projectilePrefab, beam1, beam2, mortar, parent, dropItem, pane, healthWhite, healthRed, canvas; //dropItem tillagt av Basir
    [SerializeField] private Transform player;
    [SerializeField] private AudioClip projectileSFX;
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer topSprite, botSprite;
    private List<GameObject> whitePanels = new List<GameObject>();
    private List<GameObject> redPanels = new List<GameObject>();

    private AudioSource audioSource;
    //[SerializeField] private Material originalMatTop, originalMatBot;
    private int beamDirection;
    private bool beamsActive = false;
    private Material topMat, botMat;
    private Coroutine combatCoroutine;
    private Coroutine whiteHealthCoroutine = null;

    private bool playerDetected;
    private bool isDead, droppedLoot = false;
    private bool canDealDamage = true;
    private static int dmgMultiplier = 1;
    void Start()
    {
        //tilldela värden på variabler
        beamDirection = 1;
        HP = 175;
        audioSource = GetComponent<AudioSource>();
        topMat = topSprite.material;
        botMat = botSprite.material;
        for (int i = 1; i < 321; i++)
        {
            GameObject panel = Instantiate(pane, healthWhite.transform);
            RectTransform rectTransform = panel.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector3((float)(-386 + i*2.5), 416.8f, 0);
            whitePanels.Add(panel);
        }
        for (int i = 1; i < 321; i++)
        {
            GameObject panel = Instantiate(pane, healthWhite.transform);
            RectTransform rectTransform = panel.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector3((float)(-386 + i * 2.5), 416.8f, 0);
            panel.GetComponent<Image>().color = Color.red;
            redPanels.Add(panel);
        }

    }

    protected override void Awake()
    {

        base.Awake(); // Make sure to always keep that line
        if (player == null)
        {
            player = IsPlayer.FindPlayerTransformAutomaticallyIfNull(); //Tillagt av Elias
        }
    }

    private IEnumerator Combat()
    {
        canvas.SetActive(true);
        StartCoroutine(UpdateWhiteHealth());
        StartCoroutine(FlashBeam(beam1, 4));
        beam1.transform.parent = transform;
        beam2.transform.parent = transform;
        yield return new WaitUntil(() => beamsActive);
        while (HP > 135)
        {

            RangedAttack(0);
            yield return new WaitForSeconds(0.3f);
            anim.SetTrigger("Shoot");
            yield return new WaitForSeconds(1.6f);
            
            RangedAttack(22.5f);
            yield return new WaitForSeconds(0.3f);
            anim.SetTrigger("Shoot");
            yield return new WaitForSeconds(1.6f);
        }
        StartCoroutine(FlashBeam(beam2, 4));
        beam1.transform.parent = transform;
        beam2.transform.parent = transform;
        while (HP > 75)
        {
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < Random.Range(3, 7); j++)
                {

                    RangedAttack(0);
                    yield return new WaitForSeconds(0.3f);
                    anim.SetTrigger("Shoot");
                    yield return new WaitForSeconds(1.6f);

                    RangedAttack(22.5f);
                    yield return new WaitForSeconds(0.3f);
                    anim.SetTrigger("Shoot");
                    yield return new WaitForSeconds(1.6f);
                }
                StartCoroutine(RotateBeams());
            }
        }
        beam1.transform.parent = transform;
        beam2.transform.parent = transform;
        while (HP > 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < Random.Range(3, 7); j++)
                    {

                        RangedAttack(0);
                        yield return new WaitForSeconds(0.3f);
                        anim.SetTrigger("Shoot");
                        yield return new WaitForSeconds(1.6f);

                        StartCoroutine(SpawnMortar());

                        RangedAttack(22.5f);
                        yield return new WaitForSeconds(0.3f);
                        anim.SetTrigger("Shoot");
                        yield return new WaitForSeconds(1.6f);
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
        beam.GetComponent<BoxCollider2D>().enabled = true;
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
            if (HP <= 0) { Destroy(mortarAim); }
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
            StoreKillerInfoInDamageGameObject(projectile);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            projectile.GetComponent<Rigidbody2D>().rotation = angle;
            projectile.GetComponent<Rigidbody2D>().velocity = direction * 0.8f;    
        }
        Invoke("PlaySFX", 0.5f);
    }
    private void PlaySFX()
    {
        audioSource.pitch = Random.Range(0.85f, 1.15f);
        audioSource.PlayOneShot(projectileSFX, 0.15f);
    }
    private void StoreKillerInfoInDamageGameObject(GameObject projectile)
    {
        BulletID bulletInfo = projectile.GetComponent<BulletID>();
        bulletInfo.KillerGameObject = gameObject;
    }
    private IEnumerator UpdateWhiteHealth()
    {
        yield return new WaitForSeconds(2.7f);
        while (redPanels.Count < whitePanels.Count)
        {
            whitePanels[whitePanels.Count - 1].SetActive(false);
            whitePanels.RemoveAt(whitePanels.Count - 1);

            yield return new WaitForSeconds(0.05f);
        }
        whiteHealthCoroutine = null;
    }
    private IEnumerator UpdateRedHealth()
    {
        while (HP / 175.0f * 321.0f < redPanels.Count)
        {
            redPanels[redPanels.Count - 1].SetActive(false);
            redPanels.RemoveAt(redPanels.Count - 1);
            yield return null;
        }
        if (whiteHealthCoroutine == null)
        {
            whiteHealthCoroutine = StartCoroutine(UpdateWhiteHealth());
        }
    }

    public override void TakeDamage()
    {
        HP -= 1 * dmgMultiplier;
        StartCoroutine(Flash());
        StartCoroutine(UpdateRedHealth());
        if (HP <= 0)
        {
            OnDied();
            if (combatCoroutine != null)
            {
                StopCoroutine(combatCoroutine);
            }
            topSprite.color = Color.red;
            botSprite.color = Color.red;
            Destroy(parent, 1f);
            canvas.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PlayerAttack"))
        {
            Destroy(other.gameObject);
            TakeDamage();
        }        
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PlasmaGunLaser") && canDealDamage)
        {
            HP -= 0.5f * dmgMultiplier;
            StartCoroutine(Flash());
            canDealDamage = false;
            Invoke("damageCooldownReset", .2f);
            StartCoroutine(UpdateRedHealth());

        }

        if (HP <= 0)
        {


            OnDied();
            isDead = true;

            if (combatCoroutine != null)
            {
                StopCoroutine(combatCoroutine);
            }
            topSprite.color = Color.red;
            botSprite.color = Color.red;
            dropLoot();
            Destroy(gameObject, 1f);
            canvas.SetActive(false);
        }
    }

    private bool hasRunned = false;
    private void OnDied()
    {
        if (hasRunned) return;
        hasRunned = true;
        if (beam1)
        {
            Destroy(beam1);
        }
        if (beam2) { Destroy(beam2); }

        SingletonClass.OnEnemyKilled(this);
        int XP_TO_AWARD_PLAYER_FOR_KILLING_ENEMY = 500;
        SingletonClass.AwardXP(XP_TO_AWARD_PLAYER_FOR_KILLING_ENEMY);
        spawnLevelPortal();
    }

    public GameObject elevator;
    private void spawnLevelPortal()
    {
        GameObject levelElevator = Instantiate(elevator, transform.position, Quaternion.identity, RoomContentGenerator.getItemParent());
                
    }

    private IEnumerator Flash()
    {
        if (isDead) yield break;
        topSprite.material = flashOnHit;
        botSprite.material = flashOnHit;
        yield return new WaitForSeconds(0.125f);
        topSprite.material = topMat;
        botSprite.material = botMat;
    }
    void FixedUpdate()
    {
        GetComponent<Rigidbody2D>().WakeUp();
        if (beamsActive)
        {
            transform.Rotate(0f, 0f, beamDirection*18f * Time.fixedDeltaTime);
        }
        if (!playerDetected)
        {
            if (IsPlayerWithinDetectionRadius())
            {
                playerDetected = true;
                combatCoroutine = StartCoroutine(Combat());
            }
        }
        
    }
    private bool IsPlayerWithinDetectionRadius()
    {
        float dist = Vector2.Distance(transform.position, player.position);
        if (dist <= 15f)
        {
            return true;
        }
        return false;
    }
    private void dropLoot()
    {
        if (droppedLoot) return;
        droppedLoot = true;
        GetComponent<LootBag>().InstantiateLoot(transform.position);
    }

 


    public override bool GetIsChasingPlayer()
    {
        return playerDetected;
    }

    private void damageCooldownReset()
    {
        canDealDamage = true;
    }

    public static void DamageMultiplier(int amount)
    {
        dmgMultiplier += amount;
    }

    public static IEnumerator TempDmgIncrease(int amount, float time)
    {
        DamageMultiplier(amount);
        Debug.Log("Damage multiplier increased to: " + dmgMultiplier);
        yield return new WaitForSeconds(time);
        DamageMultiplier(-amount);
        Debug.Log("Damage multiplier decreased to: " + dmgMultiplier);
    }

}