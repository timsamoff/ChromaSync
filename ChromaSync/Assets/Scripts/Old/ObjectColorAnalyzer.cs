using UnityEngine;

public class ObjectColorAnalyzer : MonoBehaviour
{
    [SerializeField] private int similarityThreshold = 10; // Adjust this threshold as needed
    [SerializeField] private string successMessage = "You did it!"; // Message to log when all channels are satisfied

    [Header("Object Color Channels")]
    [SerializeField] private float objectRed = 0.0f;
    [SerializeField] private float objectGreen = 0.0f;
    [SerializeField] private float objectBlue = 0.0f;

    private bool[] channelsSatisfied = new bool[3]; // Array to track satisfaction of each channel

    private void Update()
    {
        // Use the serialized fields for object color channels
        Color objectColor = new Color(objectRed / 255f, objectGreen / 255f, objectBlue / 255f);

        // Get the background color
        Color backgroundColor = Camera.main.backgroundColor;

        // Check similarity for each channel
        LogComparison("R", objectColor.r * 255f, backgroundColor.r * 255f, similarityThreshold, 0);
        LogComparison("G", objectColor.g * 255f, backgroundColor.g * 255f, similarityThreshold, 1);
        LogComparison("B", objectColor.b * 255f, backgroundColor.b * 255f, similarityThreshold, 2);

        // Check if all channels are satisfied
        if (IsAllChannelsSatisfied())
        {
            Debug.Log(successMessage);
        }
    }

    private void LogComparison(string channelName, float objectChannel, float backgroundChannel, int threshold, int channelIndex)
    {
        // Calculate the absolute difference between the object and background channel values
        float absoluteDifference = Mathf.Abs(objectChannel - backgroundChannel);

        // Check if the absolute difference is within the similarity threshold
        channelsSatisfied[channelIndex] = absoluteDifference <= threshold;

        // Log the comparison result only when the channel is satisfied
        if (channelsSatisfied[channelIndex])
        {
            Debug.Log($"Object {channelName} is similar to Background {channelName} with a threshold of {threshold}");
        }
    }

    private bool IsAllChannelsSatisfied()
    {
        // Check if all three color channels have been satisfied
        return channelsSatisfied[0] && channelsSatisfied[1] && channelsSatisfied[2];
    }
}