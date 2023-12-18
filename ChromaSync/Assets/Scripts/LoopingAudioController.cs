using UnityEngine;

public class LoopingAudioController : MonoBehaviour
{
    [SerializeField]
    private float fadeInTime = 20f; // Adjust this value for the desired fade-in time

    [SerializeField]
    private float fadeOutTime = 2f; // Adjust this value for the desired fade-out time

    [SerializeField]
    private float targetVolume = 1f; // Adjust this value for the desired target volume

    [SerializeField]
    private float logRolloffMultiplier = 4f; // Adjust this value for the starting point of the exponential fade

    private AudioSource audioSource;
    private float currentVolume;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0f; // Ensure starting volume is set to 0
        audioSource.Play(); // Start playing the audio source
        StartCoroutine(ExponentialFadeIn());
    }

    private System.Collections.IEnumerator ExponentialFadeIn()
    {
        float startTime = Time.time;
        float endTime = startTime + fadeInTime;

        while (Time.time < endTime)
        {
            float elapsed = Time.time - startTime;
            float t = elapsed / fadeInTime;
            currentVolume = Mathf.Lerp(0f, targetVolume, Mathf.Pow(t, logRolloffMultiplier)); // Exponential fade
            audioSource.volume = currentVolume;
            yield return null;
        }

        currentVolume = targetVolume;
        audioSource.volume = currentVolume;
    }

    public void StartFadeOut()
    {
        StartCoroutine(ExponentialFadeOut());
    }

    private System.Collections.IEnumerator ExponentialFadeOut()
    {
        float startTime = Time.time;
        float endTime = startTime + fadeOutTime;

        while (Time.time < endTime)
        {
            float elapsed = Time.time - startTime;
            float t = elapsed / fadeOutTime;
            currentVolume = Mathf.Lerp(targetVolume, 0f, Mathf.Pow(t, logRolloffMultiplier)); // Exponential fade
            audioSource.volume = currentVolume;
            yield return null;
        }

        currentVolume = 0f;
        audioSource.volume = currentVolume;
        audioSource.Stop();
    }
}