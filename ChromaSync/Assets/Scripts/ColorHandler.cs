/* using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Windows.Speech;

public class ColorHandler : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private float animationSpeed = 1.0f;

    private GameObject prefabInstance;
    private Material originalMaterial;
    private Color currentColor = Color.gray;

    [Header("Color Channels (0-255)")]
    [SerializeField] private int red = 0;
    [SerializeField] private int green = 0;
    [SerializeField] private int blue = 0;

    private Coroutine colorAnimationCoroutine;
    private ColorSoundManager colorSoundManager;
    private bool stopByColorConditions = false;
    private KeywordRecognizer keywordRecognizer;
    private bool isListening = false;
    private string lastCommand = null;
    private char currentAxis = 'r';

    private void Start()
    {
        prefabInstance = GetComponent<RandomPrefabSelector>().GetInstantiatedPrefab();
        originalMaterial = prefabInstance.GetComponent<Renderer>().material;

        originalMaterial.SetFloat("_Mode", 3);
        originalMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        originalMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        originalMaterial.SetInt("_ZWrite", 0);
        originalMaterial.DisableKeyword("_ALPHATEST_ON");
        originalMaterial.EnableKeyword("_ALPHABLEND_ON");
        originalMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        originalMaterial.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

        originalMaterial.color = startingColor;

        prefabInstance.GetComponent<Renderer>().material = originalMaterial;

        currentColor = startingColor;

        colorSoundManager = GetComponent<ColorSoundManager>();

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

    private void StopListening()
    {
        if (isListening)
        {
            keywordRecognizer.Stop();
            isListening = false;
            Debug.Log("Stopped listening...");
        }
    }

    private void RecognizedSpeech(PhraseRecognizedEventArgs speech)
    {
        Debug.Log($"Word Spoken: {speech.text}");

        string currentWord = speech.text;

        if (currentWord == stopKey)
        {
            StopColorAnimation();
        }
        else if (currentWord == moreKey || currentWord == lessKey)
        {
            lastCommand = currentWord;
            currentAxis = ' ';
            stopByColorConditions = false;
        }
        else if (currentWord.StartsWith(moreKey) || currentWord.StartsWith(lessKey))
        {
            if (lastCommand != null && lastCommand != stopKey)
            {
                Debug.Log($"Invalid command. Expected: stop.");
            }
            else
            {
                HandleColorCommand(currentWord);
            }
        }
        else if (lastCommand == moreKey || lastCommand == lessKey)
        {
            if (currentWord == redKey || currentWord == greenKey || currentWord == blueKey)
            {
                lastCommand += currentWord;
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
        currentAxis = color.Substring(4)[0];
        StartColorAnimation();
        PlaySound();
    }

    private void StopColorAnimation()
    {
        if (colorAnimationCoroutine != null)
        {
            StopCoroutine(colorAnimationCoroutine);
        }

        stopByColorConditions = true;
        lastCommand = stopKey;
        StartListening();
        colorSoundManager.StopAllSounds();
    }

    private IEnumerator ContinuousColorAnimation()
    {
        float increment = (lastCommand.StartsWith(moreKey)) ? 1.0f : -1.0f;
        stopByColorConditions = false;

        while (lastCommand.StartsWith(moreKey) || lastCommand.StartsWith(lessKey))
        {
            float incrementValue = increment / 255f;
            int previousRed = red;
            int previousGreen = green;
            int previousBlue = blue;

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

            Color newAlbedoColor = new Color(red / 255f, green / 255f, blue / 255f, originalMaterial.color.a);
            Debug.Log($"Setting color to: {newAlbedoColor}");
            prefabInstance.GetComponent<Renderer>().material.color = newAlbedoColor;
            currentColor = new Color(red / 255f, green / 255f, blue / 255f);
            prefabInstance.GetComponent<Renderer>().material.color = currentColor;

            if ((red == 0 || red == 255 || green == 0 || green == 255 || blue == 0 || blue == 255) && !stopByColorConditions)
            {
                colorSoundManager.StopAllSounds();
                stopByColorConditions = false;
                lastCommand = stopKey;
                yield break;
            }

            if (lastCommand == stopKey)
            {
                StopColorAnimation();
                yield break;
            }

            if (previousRed != red || previousGreen != green || previousBlue != blue)
            {
                Debug.Log($"Increment Values: R = {red}, G = {green}, B = {blue}");
                yield return new WaitForSeconds(animationSpeed / 255f);
            }
        }

        StopListening();
    }

    private void StartColorAnimation()
    {
        if (colorAnimationCoroutine != null)
        {
            StopCoroutine(colorAnimationCoroutine);
        }

        PlaySound();
        red = Mathf.RoundToInt(currentColor.r * 255f);
        green = Mathf.RoundToInt(currentColor.g * 255f);
        blue = Mathf.RoundToInt(currentColor.b * 255f);
        colorAnimationCoroutine = StartCoroutine(ContinuousColorAnimation());
    }

    private void PlaySound()
    {
        if (!stopByColorConditions)
        {
            if (lastCommand.StartsWith(moreKey))
            {
                switch (currentAxis)
                {
                    case 'r':
                        colorSoundManager.PlayRedLoopFwd();
                        break;
                    case 'g':
                        colorSoundManager.PlayGreenLoopFwd();
                        break;
                    case 'b':
                        colorSoundManager.PlayBlueLoopFwd();
                        break;
                }
            }
            else if (lastCommand.StartsWith(lessKey))
            {
                switch (currentAxis)
                {
                    case 'r':
                        colorSoundManager.PlayRedLoopRev();
                        break;
                    case 'g':
                        colorSoundManager.PlayGreenLoopRev();
                        break;
                    case 'b':
                        colorSoundManager.PlayBlueLoopRev();
                        break;
                }
            }
        }
    }

    private void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnSelectedLocaleChanged;
    }
} */
