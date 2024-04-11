using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class EnemyAI : MonoBehaviour
{
    //[SerializeField] private Material flashOnHit;
    [SerializeField] private GameObject projectilePrefab;
    //[SerializeField] private AudioClip projectileSFX;
    [SerializeField] private Transform player;

    private AudioSource audioSource;
    private Material originalMat;
    private SpriteRenderer sprd;
    private Animator anim;
    private int HP;

    void Start()
    {
        HP = 100;
        audioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        //originalMat = sprd.material;
        StartCoroutine(Combat());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //StartCoroutine(Combat());
    }

    private IEnumerator Combat()
    {
        while (HP > 0)
        {
            float dist = Vector2.Distance(transform.position, player.position);
            if (dist >= 0f)
            {

                anim.SetTrigger("RangedAttack");
                Invoke("RangedAttack", 0.5f); // Sätt tid till hur länge animationen körs
                yield return new WaitForSeconds(2.5f);
            }
            else
            {
                // MeleeAttack();
                yield return new WaitForSeconds(2.5f);
            }

        }
    }
    private void RangedAttack()
    {
        
        GameObject projectile = Instantiate(projectilePrefab, new Vector2(transform.position.x, transform.position.y+ 1f), Quaternion.identity);
        Vector2 direction = (player.position - projectile.transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        projectile.GetComponent<Rigidbody2D>().rotation = angle;
        projectile.GetComponent<Rigidbody2D>().velocity = direction * 3.5f;
    }
    private IEnumerator Flash()
    {
        //sprd.material = flashOnHit;
        yield return new WaitForSeconds(0.125f);
        sprd.material = originalMat;
    }
}
