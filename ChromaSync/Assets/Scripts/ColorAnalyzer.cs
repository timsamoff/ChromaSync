using UnityEngine;

public class ColorAnalyzer : MonoBehaviour
{
    [SerializeField] private int difficultyLevel = 10;
    [SerializeField] private AudioClip matchSound;
    [SerializeField] private AudioClip completionSound;

    private Material material;

    private int satisfiedChannelsCount = 0;
    private bool completionTriggered = false;

    private AudioSource audioSource;

    void Start()
    {
        material = GetComponent<Renderer>().material;

        // Subscribe to the background color change event
        RandomBackgroundColor.OnBackgroundColorChanged += OnBackgroundColorChanged;

        // Assuming you have an AudioSource component attached to the same GameObject
        audioSource = GetComponent<AudioSource>();
    }

    void OnDestroy()
    {
        // Unsubscribe from the background color change event to avoid memory leaks
        RandomBackgroundColor.OnBackgroundColorChanged -= OnBackgroundColorChanged;
    }

    void OnBackgroundColorChanged(Color newColor)
    {
        // Handle the background color change here
        Debug.Log("Background Color: R=" + (int)(newColor.r * 255) + ", G=" + (int)(newColor.g * 255) + ", B=" + (int)(newColor.b * 255));

        // Trigger your color analysis logic here if needed
        CheckColorChannels();
    }

    void Update()
    {
        // Other update logic if needed
    }

    void CheckColorChannels()
    {
        // Your color analysis logic goes here
        CheckColorChannel(0, material.color.r * 255, PlayFirstTwoChannels); // Check red channel
        CheckColorChannel(1, material.color.g * 255, PlayFirstTwoChannels); // Check green channel
        CheckColorChannel(2, material.color.b * 255, PlayThirdChannel); // Check blue channel
    }

    void CheckColorChannel(int channel, float currentColor, System.Action callback)
    {
        float backgroundChannel = 0f;

        // Set the background color channel based on the current channel
        switch (channel)
        {
            case 0:
                backgroundChannel = RandomBackgroundColor.CurrentBackgroundColor.r * 255;
                break;
            case 1:
                backgroundChannel = RandomBackgroundColor.CurrentBackgroundColor.g * 255;
                break;
            case 2:
                backgroundChannel = RandomBackgroundColor.CurrentBackgroundColor.b * 255;
                break;
        }

        // Debug.Log("Object Color: R=" + (int)(material.color.r * 255) + ", G=" + (int)(material.color.g * 255) + ", B=" + (int)(material.color.b * 255));

        // Check if the current color is within the acceptable range of the background color channel
        if (Mathf.Abs(currentColor - backgroundChannel) <= difficultyLevel)
        {
            // Channel satisfied, execute callback based on the channel
            if (channel == 0)
            {
                satisfiedChannelsCount = Mathf.Max(1, satisfiedChannelsCount);
                callback.Invoke();
            }
            else if (channel == 1 && satisfiedChannelsCount == 1)
            {
                satisfiedChannelsCount = Mathf.Max(2, satisfiedChannelsCount);
                callback.Invoke();
            }
            else if (channel == 2 && satisfiedChannelsCount == 2)
            {
                satisfiedChannelsCount = Mathf.Max(3, satisfiedChannelsCount);
                callback.Invoke();
            }
            else
            {
                satisfiedChannelsCount = Mathf.Max(0, satisfiedChannelsCount - 1);
            }
        }
    }

    void PlayFirstTwoChannels()
    {
        // Your logic for playing sound for the first two channels
        // Example: Play sound
        if (matchSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(matchSound);
        }
    }

    void PlayThirdChannel()
    {
        // Your logic for playing sound for the third channel
        // Example: Play sound and complete
        if (satisfiedChannelsCount == 3 && !completionTriggered)
        {
            completionTriggered = true;
            if (completionSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(completionSound);
            }
            Debug.Log("Complete!");
        }
    }

    // Other methods as needed
}