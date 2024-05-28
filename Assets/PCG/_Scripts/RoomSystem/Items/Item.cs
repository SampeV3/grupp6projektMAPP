using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Item : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private BoxCollider2D itemCollider;

    [SerializeField]
    int health = 3;
    [SerializeField]
    bool nonDestructible;
    [SerializeField] bool explodeOnDestruction;

    [SerializeField]
    private GameObject hitFeedback, destroyFeedback, explosion;

    public UnityEvent OnGetHit { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public void Initialize(ItemData itemData)
    {
        //set sprite
        spriteRenderer.sprite = itemData.sprite;
        //set sprite offset
        spriteRenderer.transform.localPosition = new Vector2(0.5f * itemData.size.x, 0.5f * itemData.size.y);
        itemCollider.size = itemData.size;
        itemCollider.offset = spriteRenderer.transform.localPosition;

        if (itemData.nonDestructible)
            nonDestructible = true;

        this.health = itemData.health;
        this.explodeOnDestruction = itemData.explodeOnDestruction;

    }

    public void GetHit(int damage, GameObject damageDealer)
    {
        if (nonDestructible)
            return;
        if(health>1)
            Destroy(Instantiate(hitFeedback, spriteRenderer.transform.position, Quaternion.identity), 2);
            
        else
            Destroy(Instantiate(destroyFeedback, spriteRenderer.transform.position, Quaternion.identity), 3);
        spriteRenderer.transform.DOShakePosition(0.2f, 0.3f, 75, 1, false, true).OnComplete(ReduceHealth);
    }

    private void ReduceHealth()
    {
        health--;
        if (health <= 0)
        {
            if (explodeOnDestruction)
            {
                Instantiate(explosion, spriteRenderer.transform.position, spriteRenderer.transform.rotation, RoomContentGenerator.getItemParent());
            }

            spriteRenderer.transform.DOComplete();
            Destroy(gameObject);
        }       
    }
    private bool canDealDamage = true;

    private void damageCooldownReset()
    {
        canDealDamage = true;

    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PlasmaGunLaser") && canDealDamage)
        {
            canDealDamage = false;
            Invoke("damageCooldownReset", .2f);
            GetHit(1, other.gameObject);
        }


    }


}

