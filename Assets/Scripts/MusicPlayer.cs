using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Music : MonoBehaviour
{
    private static int instancesOfClass = 0;

    public double goalTime;
    public double musicDuration;


    public bool tryCombatMusic = false;
    public bool trySong = true;
    public int audioToggle;


    public AudioSource introAudioSource , loopAudioSource, combatAudioSource;
    public AudioClip song;

    void Awake()
    {
        if (instancesOfClass < 1)
        {
            instancesOfClass++;
            DontDestroyOnLoad(gameObject.transform.parent.gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {
        if (trySong)
        {
            loopAudioSource.clip = song;
            loopAudioSource.Play();
            return;
        }

        double introDuration = (double)introAudioSource.clip.samples / introAudioSource.clip.frequency;
        double startTime = AudioSettings.dspTime + 0.2;
        introAudioSource.PlayScheduled(startTime);
        loopAudioSource.PlayScheduled(startTime + introDuration);
        //DontDestroyOnLoad(this.gameObject);
    }

    void OnCombat(bool isInCombatMode, string situation)
    {
        

        if (!tryCombatMusic) {return;}
        if (isInCombatMode)
        {
            loopAudioSource.DOFade(0, 1f);
            introAudioSource.DOFade(0, 1f);
            combatAudioSource.DOFade(0.5f, 1f);
            combatAudioSource.PlayScheduled(1f);
        }
        else
        {
            combatAudioSource.DOFade(0, 1f);
            loopAudioSource.DOFade(1, 1f);
            introAudioSource.DOFade(1, 1f);
        }
    }

    private void OnEnable()
    {
        PlayerTakeDamage.OnCombatSituationChanged += OnCombat;
    }

    private void OnDisable()
    {
        PlayerTakeDamage.OnCombatSituationChanged -= OnCombat;
    }
}
