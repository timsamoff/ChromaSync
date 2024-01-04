using UnityEngine;

public class ColorAnalyzer : MonoBehaviour
{
    [SerializeField] private int difficultyLevel = 10;
    [SerializeField] private AudioClip redMatchSound;
    [SerializeField] private AudioClip greenMatchSound;
    [SerializeField] private AudioClip blueMatchSound;
    [SerializeField] private AudioClip completionSound;

    private bool completionTriggered = false;

    private AudioSource audioSource;

    // Expose this property to display material albedo color in the Inspector
    [ReadOnly][SerializeField] private Color materialAlbedoColor;

    // Array to track whether the sound for each channel has been played since the last satisfaction
    private bool[] soundPlayedThisSatisfaction = new bool[3];

    // Count for keeping track of the number of channels satisfied
    private int[] channelsSatisfiedCount = new int[3];

    void Start()
    {
        // Assuming you have an AudioSource component attached to the same GameObject
        audioSource = GetComponent<AudioSource>();

        // Call the method to update materialAlbedoColor initially
        UpdateMaterialAlbedoColor();
    }

    void Update()
    {
        // Continually monitor and compare colors in real-time
        MonitorAndCompareColors();
    }

    void MonitorAndCompareColors()
    {
        // Check red channel
        CheckColorChannel(0, materialAlbedoColor.r * 255, redMatchSound);
        // Check green channel
        CheckColorChannel(1, materialAlbedoColor.g * 255, greenMatchSound);
        // Check blue channel
        CheckColorChannel(2, materialAlbedoColor.b * 255, blueMatchSound);

        // Update materialAlbedoColor in real-time
        UpdateMaterialAlbedoColor();
    }

    float GetBackgroundChannel(int channel)
    {
        return RandomBackgroundColor.CurrentBackgroundColor[channel] * 255;
    }

    float CalculateThreshold(int channel, float backgroundChannel)
    {
        // Calculate the threshold based on the difficultyLevel
        return difficultyLevel > 0 ? difficultyLevel : 1; // Ensure threshold is at least 1
    }

    void UpdateMaterialAlbedoColor()
    {
        // Assuming the material is directly on the object
        if (TryGetComponent<Renderer>(out Renderer renderer) && renderer.material != null)
        {
            materialAlbedoColor = renderer.material.color;
        }
        else
        {
            Debug.LogError("The object must have a Renderer component with a material assigned.");
        }
    }

    void CheckColorChannel(int channel, float currentColor, AudioClip matchSound)
    {
        if (completionTriggered)
        {
            // If completion is triggered, stop checking
            return;
        }

        string channelName = GetChannelName(channel);
        float backgroundChannel = GetBackgroundChannel(channel);
        float threshold = CalculateThreshold(channel, backgroundChannel);

        // Log relevant values for debugging
        Debug.Log($"{channelName} - Current Color: {currentColor}, Background Channel: {backgroundChannel}, Threshold: {threshold}");

        // Check if the current color is within the acceptable range of the background color channel
        if (Mathf.Abs(currentColor - backgroundChannel) <= threshold)
        {
            Debug.Log($"{channelName} satisfied!");

            // Play matchSound audio whenever a channel becomes satisfied
            if (matchSound != null && audioSource != null && !soundPlayedThisSatisfaction[channel])
            {
                audioSource.PlayOneShot(matchSound);
                soundPlayedThisSatisfaction[channel] = true; // Mark sound as played for this satisfaction
            }

            // Increment the count of satisfied channels for this color
            channelsSatisfiedCount[channel]++;

            // Check if all three channels are satisfied
            if (channelsSatisfiedCount[0] > 0 && channelsSatisfiedCount[1] > 0 && channelsSatisfiedCount[2] > 0 && !completionTriggered)
            {
                completionTriggered = true;

                // Play completionSound audio
                if (completionSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(completionSound);
                    // Call the new function (commented pseudo code)
                    // YourFunction();
                    Time.timeScale = 0;
                }
                Debug.Log("Complete!");
            }
        }
        else
        {
            // If the condition is not satisfied, reset the count and soundPlayedThisSatisfaction for this color
            channelsSatisfiedCount[channel] = 0;
            soundPlayedThisSatisfaction[channel] = false;
        }
    }

    string GetChannelName(int channel)
    {
        // Convert channel index to channel name
        switch (channel)
        {
            case 0: return "Red";
            case 1: return "Green";
            case 2: return "Blue";
            default: return "Unknown";
        }
    }
}

// Custom attribute to make a field read-only in the Inspector
public class ReadOnlyAttribute : PropertyAttribute { }