using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Sfx Settings")]
    [SerializeField, Range(0f, 1f)] private float SfxFwdVolume = 1.0f; // Adjust this variable for Fwd sound volume
    [SerializeField, Range(0f, 1f)] private float SfxRevVolume = 1.0f; // Adjust this variable for Rev sound volume
    [SerializeField] private float fadeTime = 1.0f; // Adjust this variable for fade in/out time

    [Header("Sound Clips")]
    [SerializeField] private AudioClip redLoopFwd;
    [SerializeField] private AudioClip greenLoopFwd;
    [SerializeField] private AudioClip blueLoopFwd;

    [SerializeField] private AudioClip redLoopRev;
    [SerializeField] private AudioClip greenLoopRev;
    [SerializeField] private AudioClip blueLoopRev;

    private AudioSource audioSource;
    private AudioSource crossfadeSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;

        // Create a child GameObject for the crossfade source
        GameObject crossfadeObject = new GameObject("CrossfadeAudioSource");
        crossfadeObject.transform.parent = transform;
        crossfadeSource = crossfadeObject.AddComponent<AudioSource>();
        crossfadeSource.playOnAwake = false;
    }

    public void PlayRedLoopFwd()
    {
        Crossfade(redLoopFwd, SfxFwdVolume);
    }

    public void PlayGreenLoopFwd()
    {
        Crossfade(greenLoopFwd, SfxFwdVolume);
    }

    public void PlayBlueLoopFwd()
    {
        Crossfade(blueLoopFwd, SfxFwdVolume);
    }

    public void PlayRedLoopRev()
    {
        Crossfade(redLoopRev, SfxRevVolume);
    }

    public void PlayGreenLoopRev()
    {
        Crossfade(greenLoopRev, SfxRevVolume);
    }

    public void PlayBlueLoopRev()
    {
        Crossfade(blueLoopRev, SfxRevVolume);
    }

    public void StopAllSounds()
    {
        StopAllCoroutines();
        StopAndFadeOut();
    }

    private void Crossfade(AudioClip newClip, float targetVolume)
    {
        StartCoroutine(FadeOutAndIn(newClip, targetVolume));
    }

    private void StopAndFadeOut()
    {
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float startVolume = audioSource.volume;
        float elapsedTime = 0f;

        while (elapsedTime < fadeTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0f, EaseInOutQuad(elapsedTime / fadeTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();
        audioSource.loop = false; // Disable looping when stopping
    }

    private IEnumerator FadeOutAndIn(AudioClip newClip, float targetVolume)
    {
        crossfadeSource.clip = newClip;
        crossfadeSource.volume = 0f;
        crossfadeSource.Play();

        float elapsedTime = 0f;

        while (elapsedTime < fadeTime)
        {
            // Fade out the main audio source
            audioSource.volume = Mathf.Lerp(1f, 0f, EaseInOutQuad(elapsedTime / fadeTime));

            // Fade in the crossfade source
            crossfadeSource.volume = Mathf.Lerp(0f, targetVolume, EaseInOutQuad(elapsedTime / fadeTime));

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Stop the main audio source
        audioSource.Stop();
        audioSource.volume = targetVolume;

        // Set the new sound to the main audio source
        audioSource.clip = newClip;
        audioSource.loop = true;
        audioSource.Play();

        // Stop the crossfade source
        crossfadeSource.Stop();
    }

    private float EaseInOutQuad(float t)
    {
        // Easing function: quadratic ease-in-out
        return t < 0.5f ? 2f * t * t : -1f + (4f - 2f * t) * t;
    }
}