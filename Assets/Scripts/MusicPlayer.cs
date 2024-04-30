using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Music : MonoBehaviour
{

    public double goalTime;
    public double musicDuration;

    public bool tryCombatMusic = false;
    public int audioToggle;

    public AudioSource introAudioSource , loopAudioSource, combatAudioSource;

    void Start() {
        double introDuration = (double)introAudioSource.clip.samples / introAudioSource.clip.frequency;
        double startTime = AudioSettings.dspTime + 0.2;
        introAudioSource.PlayScheduled(startTime);
        loopAudioSource.PlayScheduled(startTime + introDuration);
        //DontDestroyOnLoad(this.gameObject);
    }

    void OnCombat(bool isInCombatMode, string situation)
    {
        //bara en idé på combat mode music, men sättet eventet triggas behöver bli mer tillförlitligt först :)
        //annars kan musiken stanna ex. mitt i en fight plötsligt i 1 - 2 sekunder
        //dessutom måste nog båda låtarna matcha varandra, så att de inte skiljer sig så mycket och det blir för mycket dissonans?
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
