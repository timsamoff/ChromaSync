using System.Collections;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public float fadeTime = 2.0f;

    void Start()
    {
        // Access the AudioSource component directly from the GameObject this script is attached to.
        AudioSource backgroundMusic = GetComponent<AudioSource>();

        // Start the music with zero volume and then gradually increase it.
        backgroundMusic.volume = 0f;
        StartCoroutine(FadeInMusic(backgroundMusic));
    }

    IEnumerator FadeInMusic(AudioSource audioSource)
    {
        float currentTime = 0f;

        while (currentTime < fadeTime)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, 1f, currentTime / fadeTime);
            yield return null;
        }

        // Ensure the volume is set to 1 at the end to avoid rounding errors.
        audioSource.volume = 1f;
    }
}