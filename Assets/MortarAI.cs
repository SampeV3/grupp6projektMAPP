using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class MortarAI : MonoBehaviour
{
    [SerializeField] private Material flashOnHit;
    [SerializeField] private GameObject mortarIndicator;
    [SerializeField] private Transform player;
    //[SerializeField] private AudioClip projectileSFX;
    
    private Animator anim;
    private AudioSource audioSource;
    private SpriteRenderer sprd;
    private Material originalMat;
    private int HP;
    void Start()
    {
        HP = 10;
        audioSource = GetComponent<AudioSource>();
        sprd = GetComponent<SpriteRenderer>();
        originalMat = sprd.material;
        StartCoroutine(Combat());
    }

    private IEnumerator Combat()
    {
        while (HP > 0)
        {
            //anim.SetTrigger("Attack");
            StartCoroutine(SpawnMortar());
            yield return new WaitForSeconds(2.5f);
        }
        Destroy(gameObject);
    }

    private IEnumerator SpawnMortar()
    {
        GameObject mortarAim = Instantiate(mortarIndicator, player.position, Quaternion.identity);
        Transform desiredChild = mortarAim.transform.Find("MortarBackground");
        SpriteRenderer mortarsprd = desiredChild.GetComponent<SpriteRenderer>();
        Color ogColor = mortarsprd.color;
        ogColor.a = 0.1f;
        mortarsprd.color = ogColor;
        while (mortarsprd.color.a < 1f)
        {
            mortarAim.transform.Rotate(0f, 0f, 15f * Time.fixedDeltaTime);
            ogColor.a += 0.5f * Time.deltaTime;
            mortarsprd.color = ogColor;
            yield return null;
        }
        Destroy(mortarAim);
    }
   
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PlayerAttack"))
        {
            Destroy(other.gameObject);
            HP -= 3;
            StartCoroutine(Flash());
        }
    }

    private IEnumerator Flash()
    {
        sprd.material = flashOnHit;
        yield return new WaitForSeconds(0.125f);
        sprd.material = originalMat;
    }
}