using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionDamage : MonoBehaviour
{
    private CircleCollider2D circleCollider;
    private Animator animator;
    [SerializeField] private AudioClip explosionAudio;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.enabled = true;
        circleCollider = GetComponent<CircleCollider2D>();
        if (RoomContentGenerator.getItemParent().TryGetComponent<AudioSource>(out AudioSource audioSource)) {
            audioSource.PlayOneShot(explosionAudio);
        }
        Destroy(gameObject, 2);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<EnemyMonoBehaviour>(out EnemyMonoBehaviour enemy)){
            enemy.TakeDamage();
        }
        if (collision.gameObject.TryGetComponent<PlayerTakeDamage>(out PlayerTakeDamage playerTakeDamage)) {
            playerTakeDamage.TakeDamage(1, circleCollider);
        }
    }
}