using System.Collections;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class VoiceDetection : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private float animationSpeed = 1.0f;
    [SerializeField] private GameObject gObj;
    private Material originalMaterial;
    private Color currentColor = Color.black;

    [Header("Color Channels (0-255)")]
    [SerializeField] private int red = 0;
    [SerializeField] private int green = 0;
    [SerializeField] private int blue = 0;

    private KeywordRecognizer keywordRecognizer;
    private bool isListening = false;
    private string lastCommand = null;
    private char currentAxis = 'r'; // Default to red

    private Coroutine colorAnimationCoroutine;

    // Reference to the SoundManager script
    private SoundManager soundManager;

    private void Start()
    {
        originalMaterial = new Material(Shader.Find("Standard"));
        originalMaterial.color = Color.black;

        gObj.GetComponent<Renderer>().material = originalMaterial;

        keywordRecognizer = new KeywordRecognizer(new string[] { "morered", "lessred", "moregreen", "lessgreen", "moreblue", "lessblue", "more", "less", "red", "green", "blue", "stop", "top", "opp" });
        keywordRecognizer.OnPhraseRecognized += RecognizedSpeech;

        // Get the SoundManager script attached to this GameObject
        soundManager = GetComponent<SoundManager>();

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

        if (currentWord == "stop" || currentWord == "top" || currentWord == "opp")
        {
            StopColorAnimation();
        }
        else if (currentWord == "more" || currentWord == "less")
        {
            lastCommand = currentWord;
            currentAxis = ' '; // Reset currentAxis to allow color to be selected in the next command
        }
        else if (currentWord.StartsWith("more") || currentWord.StartsWith("less"))
        {
            // Handle single keywords "morered," "lessred," etc.
            if (lastCommand != null && lastCommand != "stop" && lastCommand != "top" && lastCommand != "opp")
            {
                Debug.Log($"Invalid command. Expected: stop, top, opp.");
            }
            else
            {
                HandleColorCommand(currentWord);
            }
        }
        else if (lastCommand == "more" || lastCommand == "less")
        {
            // Only allow "red", "green", or "blue" after "more" or "less"
            if (currentWord == "red" || currentWord == "green" || currentWord == "blue")
            {
                lastCommand += currentWord; // Combine "more" or "less" with the color
                currentAxis = currentWord[0];
                StartColorAnimation();
            }
            else
            {
                Debug.Log($"Invalid command after {lastCommand}. Expected: red, green, blue.");
            }
        }
    }

    private void HandleColorCommand(string color)
    {
        lastCommand = color;
        currentAxis = color.Substring(4)[0]; // Extract color channel from "morered," "lessred," etc.
        StartColorAnimation();

        // Play the corresponding sound based on the color command
        PlaySound();
    }

    private void StopColorAnimation()
    {
        if (colorAnimationCoroutine != null)
        {
            StopCoroutine(colorAnimationCoroutine);
        }

        // Require saying "stop" after "more" or "less" followed by the color
        lastCommand = "stop";

        // Continue listening after the animation stops
        StartListening();

        // Stop all sounds
        soundManager.StopAll();
    }

    private IEnumerator ContinuousColorAnimation()
    {
        float increment = (lastCommand.StartsWith("more")) ? 1.0f : -1.0f;

        while (lastCommand.StartsWith("more") || lastCommand.StartsWith("less"))
        {
            float incrementValue = increment / 255f;

            switch (currentAxis)
            {
                case 'r':
                    red += Mathf.RoundToInt(incrementValue * 255f);
                    red = Mathf.Clamp(red, 0, 255);
                    break;
                case 'g':
                    green += Mathf.RoundToInt(incrementValue * 255f);
                    green = Mathf.Clamp(green, 0, 255);
                    break;
                case 'b':
                    blue += Mathf.RoundToInt(incrementValue * 255f);
                    blue = Mathf.Clamp(blue, 0, 255);
                    break;
                case ' ':
                    break;
            }

            currentColor = new Color(red / 255f, green / 255f, blue / 255f);

            gObj.GetComponent<Renderer>().material.color = currentColor;

            Debug.Log($"Increment Values: R = {red}, G = {green}, B = {blue}");

            yield return new WaitForSeconds(animationSpeed / 255f);
        }

        // Stop listening after the animation stops
        StopListening();
    }

    private void StartColorAnimation()
    {
        if (colorAnimationCoroutine != null)
        {
            // Stop the existing coroutine if it is running
            StopCoroutine(colorAnimationCoroutine);
        }

        // Start a new coroutine for continuous color animation
        colorAnimationCoroutine = StartCoroutine(ContinuousColorAnimation());
    }

    private void PlaySound()
    {
        // Play the corresponding sound based on color and direction
        if (lastCommand.StartsWith("more"))
        {
            switch (currentAxis)
            {
                case 'r':
                    soundManager.PlayRedLoopFwd();
                    break;
                case 'g':
                    soundManager.PlayGreenLoopFwd();
                    break;
                case 'b':
                    soundManager.PlayBlueLoopFwd();
                    break;
            }
        }
        else if (lastCommand.StartsWith("less"))
        {
            switch (currentAxis)
            {
                case 'r':
                    soundManager.PlayRedLoopRev();
                    break;
                case 'g':
                    soundManager.PlayGreenLoopRev();
                    break;
                case 'b':
                    soundManager.PlayBlueLoopRev();
                    break;
            }
        }
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