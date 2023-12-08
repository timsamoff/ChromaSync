using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class VoiceDetection : MonoBehaviour
{
    [SerializeField] private float animationSpeed = 1.0f;
    [SerializeField] private float waitTime = 0.1f;

    [Header("Color Channels (0-255)")]
    [SerializeField] private int red = 0;
    [SerializeField] private int green = 0;
    [SerializeField] private int blue = 0;

    private KeywordRecognizer keywordRecognizer;
    private bool isListening = false;

    [SerializeField] private GameObject gObj;
    private Material originalMaterial;
    private Color currentColor = Color.black;

    private void Start()
    {
        originalMaterial = new Material(Shader.Find("Standard"));
        originalMaterial.color = Color.black;

        gObj.GetComponent<Renderer>().material = originalMaterial;

        keywordRecognizer = new KeywordRecognizer(new string[] { "morered", "lessred", "moregreen", "lessgreen", "moreblue", "lessblue", "more", "less", "red", "green", "blue" });
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

        float increment = 10.0f; // Default increment value

        if (currentWord.StartsWith("less"))
        {
            increment = -10.0f;
        }

        if (currentWord.Contains("red"))
        {
            IncrementColor('r', increment);
        }
        else if (currentWord.Contains("green"))
        {
            IncrementColor('g', increment);
        }
        else if (currentWord.Contains("blue"))
        {
            IncrementColor('b', increment);
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
    }

    private void IncrementColor(char axis, float increment)
    {
        StartCoroutine(ChangeColorAnimation(axis, increment));
    }

    private IEnumerator ChangeColorAnimation(char axis, float increment)
    {
        float duration = animationSpeed;
        float elapsedTime = 0f;

        float incrementValue = increment / 255f;

        switch (axis)
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