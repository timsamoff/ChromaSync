using System.Collections;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class VoiceDetection_v2 : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private float animationSpeed = 1.0f;
    [SerializeField] private float waitTime = 0.1f;
    [SerializeField] private float incrementValue = 10.0f; // Default incrementValue value
    [SerializeField] private GameObject gObj;

    [Header("Color Channels (0-255)")]
    [SerializeField] private int red = 0;
    [SerializeField] private int green = 0;
    [SerializeField] private int blue = 0;

    private KeywordRecognizer keywordRecognizer;
    private bool isListening = false;
    private bool isChangingColor = false; // Flag to track whether color changes are in progress
    private int changeDirection = 1; // 1 for increase, -1 for decrease

    private Material originalMaterial;
    private Color currentColor = Color.black;

    private void Start()
    {
        originalMaterial = new Material(Shader.Find("Standard"));
        originalMaterial.color = Color.black;

        gObj.GetComponent<Renderer>().material = originalMaterial;

        keywordRecognizer = new KeywordRecognizer(new string[] { "morered", "lessred", "moregreen", "lessgreen", "moreblue", "lessblue", "more", "less", "red", "green", "blue", "stop", "top", "opp" });
        keywordRecognizer.OnPhraseRecognized += RecognizedSpeech;

        StartListening();
    }

    private void StartListening()
    {
        if (!isListening)
        {
            keywordRecognizer.Start();
            isListening = true;
            Debug.Log("Listening for keywords...");
        }
    }

    private void RecognizedSpeech(PhraseRecognizedEventArgs speech)
    {
        Debug.Log($"Word Spoken: {speech.text}");

        string currentWord = speech.text;

        // Set the change direction based on the keyword recognized
        if (currentWord.StartsWith("less"))
        {
            changeDirection = -1; // Set the change direction to decrease
        }
        else
        {
            changeDirection = 1; // Set the change direction to increase
        }

        // Apply the change direction directly to incrementValue
        float adjustedIncrement = incrementValue * changeDirection;

        if (currentWord.Contains("red"))
        {
            StartColorChange('r', adjustedIncrement);
        }
        else if (currentWord.Contains("green"))
        {
            StartColorChange('g', adjustedIncrement);
        }
        else if (currentWord.Contains("blue"))
        {
            StartColorChange('b', adjustedIncrement);
        }
        else if (currentWord == "more")
        {
            // Handle "more" separately
            Debug.Log("Do something for 'more'...");
        }
        else if (currentWord == "less")
        {
            // Handle "less" separately
            Debug.Log("Do something for 'less'...");
        }
        else if (currentWord == "stop" || currentWord == "top" || currentWord == "opp")
        {
            StopColorChange();
        }
    }

    private void StartColorChange(char axis, float increment)
    {
        if (!isChangingColor)
        {
            isChangingColor = true;
            StartCoroutine(ChangeColorContinuously(axis, increment));
        }
    }

    private void StopColorChange()
    {
        if (isChangingColor)
        {
            isChangingColor = false;
            Debug.Log("Color change stopped.");
        }
    }

    private IEnumerator ChangeColorContinuously(char axis, float increment)
    {
        while (isChangingColor)
        {
            yield return StartCoroutine(ChangeColorAnimation(axis, increment));
            yield return null; // Ensure a frame is rendered before the next iteration
        }
    }

    private IEnumerator ChangeColorAnimation(char axis, float increment)
    {
        float duration = animationSpeed;
        float elapsedTime = 0f;

        float adjustmentValue = increment / 255f * changeDirection;

        switch (axis)
        {
            case 'r':
                red += Mathf.RoundToInt(adjustmentValue * 255f);
                red = Mathf.Clamp(red, 0, 255);
                break;
            case 'g':
                green += Mathf.RoundToInt(adjustmentValue * 255f);
                green = Mathf.Clamp(green, 0, 255);
                break;
            case 'b':
                blue += Mathf.RoundToInt(adjustmentValue * 255f);
                blue = Mathf.Clamp(blue, 0, 255);
                break;
        }

        currentColor = new Color(red / 255f, green / 255f, blue / 255f);

        Color initialColor = gObj.GetComponent<Renderer>().material.color;
        Color targetColor = new Color(red / 255f, green / 255f, blue / 255f);

        while (elapsedTime < duration)
        {
            gObj.GetComponent<Renderer>().material.color = Color.Lerp(initialColor, targetColor, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        gObj.GetComponent<Renderer>().material.color = targetColor;

        Debug.Log($"Increment Values: R = {red}, G = {green}, B = {blue}");

        yield return new WaitForSeconds(waitTime);

        StopListening();

        yield return new WaitForSeconds(waitTime);

        StartListening();
    }

    private void StopListening()
    {
        if (isListening)
        {
            keywordRecognizer.Stop();
            isListening = false;
            Debug.Log("Stopped listening...");
        }
    }
}