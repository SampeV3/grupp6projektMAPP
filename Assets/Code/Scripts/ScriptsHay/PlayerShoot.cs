using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{

    public Transform firePoint1, firePoint2; //ändrats av Basir
    public GameObject bulletPrefab;

    UIController uIController;

    public delegate void OnShootAction();
    public static event OnShootAction OnShoot;

    private void Start()
    {
        //firePoint1
        uIController = GameObject.FindGameObjectWithTag("UIController").gameObject.GetComponent<UIController>(); //tillagt av Basir
    }

    public void Shoot()  //ändrats av Basir
    {
        if (uIController.weapon_1_Selected)
        {
            Instantiate(bulletPrefab, firePoint1.position, firePoint1.rotation);
        }
        
        

        if (OnShoot != null) OnShoot();
    }

}
