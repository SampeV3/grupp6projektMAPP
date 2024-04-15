using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{

    public Transform firePoint;
    public GameObject bulletPrefab;


    public void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }

}
