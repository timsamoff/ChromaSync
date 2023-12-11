using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip redLoopFwd;
    [SerializeField] private AudioClip greenLoopFwd;
    [SerializeField] private AudioClip blueLoopFwd;

    [SerializeField] private AudioClip redLoopRev;
    [SerializeField] private AudioClip greenLoopRev;
    [SerializeField] private AudioClip blueLoopRev;

    [SerializeField] private float fadeTime = 1.0f; // Adjust this variable for fade in/out time

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

    private void Crossfade(AudioClip newClip)
    {
        StartCoroutine(FadeOutAndIn(newClip));
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
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsedTime / fadeTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();
        audioSource.loop = false; // Disable looping when stopping
    }

    private IEnumerator FadeOutAndIn(AudioClip newClip)
    {
        crossfadeSource.clip = newClip;
        crossfadeSource.volume = 0f;
        crossfadeSource.Play();

        float elapsedTime = 0f;

        while (elapsedTime < fadeTime)
        {
            // Fade out the main audio source
            audioSource.volume = Mathf.Lerp(1f, 0f, elapsedTime / fadeTime);

            // Fade in the crossfade source
            crossfadeSource.volume = Mathf.Lerp(0f, 1f, elapsedTime / fadeTime);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Stop the main audio source
        audioSource.Stop();
        audioSource.volume = 1f;

        // Set the new sound to the main audio source
        audioSource.clip = newClip;
        audioSource.loop = true;
        audioSource.Play();

        // Stop the crossfade source
        crossfadeSource.Stop();
    }

    public void PlayRedLoopFwd()
    {
        Crossfade(redLoopFwd);
    }

    public void PlayGreenLoopFwd()
    {
        Crossfade(greenLoopFwd);
    }

    public void PlayBlueLoopFwd()
    {
        Crossfade(blueLoopFwd);
    }

    public void PlayRedLoopRev()
    {
        Crossfade(redLoopRev);
    }

    public void PlayGreenLoopRev()
    {
        Crossfade(greenLoopRev);
    }

    public void PlayBlueLoopRev()
    {
        Crossfade(blueLoopRev);
    }

    public void StopAll()
    {
        StopAndFadeOut();
    }
}