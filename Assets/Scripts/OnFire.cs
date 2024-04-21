using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnFire : MonoBehaviour
{

    public AudioClip OnFireSound;
    public AudioSource player;

    void OnEnable()
    {
        PlayerShoot.OnShoot += ShotFired;
    }


    void OnDisable()
    {
        PlayerShoot.OnShoot -= ShotFired;
    }

    private void Awake()
    {
        player = GetComponent<AudioSource>();
    }

    private void ShotFired()
    {
        if (!player.enabled) return; //avoid can not play a disabled audio source warning!
        player.Play();

    }

}
