using System.Collections;
using UnityEngine;

public class ColorSoundManager : MonoBehaviour
{
    [Header("Sfx Settings")]
    [SerializeField, Range(0f, 1f)] private float SfxFwdVolume = 1.0f;
    [SerializeField, Range(0f, 1f)] private float SfxRevVolume = 1.0f;
    [SerializeField] private float fadeTime = 1.0f;
    [SerializeField, Range(0.1f, 5f)] private float logarithmicRolloff = 2.0f;
    [SerializeField, Range(0.1f, 10f)] private float decibelAdjustment = 20f;

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
            float t = elapsedTime / fadeTime;
            float linearVolume = Mathf.Lerp(startVolume, 0f, Mathf.Pow(t, logarithmicRolloff));
            float adjustedVolume = DecibelToLinear(LinearToDecibel(linearVolume) - decibelAdjustment);

            audioSource.volume = adjustedVolume;
            elapsedTime += Time.deltaTime;

            // Ensure the volume reaches zero before stopping
            if (adjustedVolume <= 0.001f)
            {
                break;
            }

            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();
        audioSource.loop = false;
    }


    private IEnumerator FadeOutAndIn(AudioClip newClip, float targetVolume)
    {
        crossfadeSource.clip = newClip;
        crossfadeSource.volume = 0f;
        crossfadeSource.Play();

        float elapsedTime = 0f;

        while (elapsedTime < fadeTime)
        {
            float t = elapsedTime / fadeTime;
            float linearFadeOut = Mathf.Lerp(1f, 0f, Mathf.Pow(t, logarithmicRolloff));
            float adjustedFadeOut = DecibelToLinear(LinearToDecibel(linearFadeOut) - decibelAdjustment);

            audioSource.volume = adjustedFadeOut;
            crossfadeSource.volume = Mathf.Lerp(0f, targetVolume, Mathf.Pow(t, logarithmicRolloff));

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = targetVolume;
        audioSource.clip = newClip;
        audioSource.loop = true;
        audioSource.Play();

        crossfadeSource.Stop();
    }

    private float LinearToDecibel(float linear)
    {
        return 20f * Mathf.Log10(linear);
    }

    private float DecibelToLinear(float decibel)
    {
        return Mathf.Pow(10f, decibel / 20f);
    }
}