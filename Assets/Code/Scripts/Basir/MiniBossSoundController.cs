using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBossSoundController : MonoBehaviour
{
    [SerializeField] private GameObject miniBoss;
    private AudioSource audioSource;
    private float distanceToMiniBoss;
    private float volume;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            audioSource.enabled = true;
            StartCoroutine(AdjustVolume(other.gameObject));

        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            audioSource.enabled = false;
        }
    }
    private IEnumerator AdjustVolume(GameObject other)
    {
        while (true)
        {
            if (audioSource.isPlaying)
            {
                distanceToMiniBoss = Vector3.Distance(miniBoss.transform.position, other.transform.position);
            }
            audioSource.volume = 2f / distanceToMiniBoss;
            volume = audioSource.volume;
            Debug.Log("Mini Boss Audio Volume: " + volume + " , Distance To Player: " + distanceToMiniBoss);
            yield return new WaitForSeconds(0.5f);
        }
    }
}
