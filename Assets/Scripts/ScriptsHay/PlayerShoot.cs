using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{

    public Transform firePoint;
    public GameObject bulletPrefab;

    public delegate void OnShootAction();
    public static event OnShootAction OnShoot;

    public void Shoot()
    {
        
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        OnShoot();
    }

}
