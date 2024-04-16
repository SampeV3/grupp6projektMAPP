using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{

    public double goalTime;
    public double musicDuration;

    public int audioToggle;

    public AudioSource introAudioSource , loopAudioSource;

    void Start() {
        double introDuration = (double)introAudioSource.clip.samples / introAudioSource.clip.frequency;
        double startTime = AudioSettings.dspTime + 0.2;
        introAudioSource.PlayScheduled(startTime);
        loopAudioSource.PlayScheduled(startTime + introDuration);
    }


}
