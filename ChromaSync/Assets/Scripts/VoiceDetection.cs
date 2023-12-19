using System.Collections;
using UnityEngine;
using UnityEngine.Windows.Speech;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;

public class VoiceDetection : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private float animationSpeed = 1.0f;
    // [SerializeField] private GameObject prefabPrefab; // Serialized field for the prefab

    [SerializeField] private Color startingColor = Color.gray;

    [SerializeField] private RandomPrefabSelector prefabSelector; // Reference to the script attached to the empty GameObject

    private GameObject prefabInstance; // Reference to the instantiated prefab
    private Material originalMaterial;
    private Color currentColor = Color.gray;

    [Header("Color Channels (0-255)")]
    [SerializeField] private int red = 0;
    [SerializeField] private int green = 0;
    [SerializeField] private int blue = 0;

    private KeywordRecognizer keywordRecognizer;
    private bool isListening = false;
    private string lastCommand = null;
    private char currentAxis = 'r'; // Default to red

    private Coroutine colorAnimationCoroutine;

    // Reference to the ColorSoundManager script
    private ColorSoundManager colorSoundManager;
    private bool stopByColorConditions = false;


    // Localization logic
    private string moreKey, lessKey, stopKey, redKey, greenKey, blueKey, moreRedKey, moreGreenKey, moreBlueKey, lessRedKey, lessGreenKey, lessBlueKey;

    private void Awake()
    {
        UpdateLocalizedKeys();

        // Subscribe to the event to update keys when the locale changes
        LocalizationSettings.SelectedLocaleChanged += OnSelectedLocaleChanged;
    }

    private void OnSelectedLocaleChanged(Locale newLocale)
    {
        UpdateLocalizedKeys();
    }

    private void UpdateLocalizedKeys()
    {
        Locale currentSelectedLocale = LocalizationSettings.SelectedLocale;

        moreKey = LocalizationSettings.StringDatabase.GetLocalizedString("VoiceCommandLocalization", "more");
        lessKey = LocalizationSettings.StringDatabase.GetLocalizedString("VoiceCommandLocalization", "less");
        stopKey = LocalizationSettings.StringDatabase.GetLocalizedString("VoiceCommandLocalization", "stop");
        redKey = LocalizationSettings.StringDatabase.GetLocalizedString("VoiceCommandLocalization", "red");
        greenKey = LocalizationSettings.StringDatabase.GetLocalizedString("VoiceCommandLocalization", "green");
        blueKey = LocalizationSettings.StringDatabase.GetLocalizedString("VoiceCommandLocalization", "blue");
        moreRedKey = LocalizationSettings.StringDatabase.GetLocalizedString("VoiceCommandLocalization", "morered");
        moreGreenKey = LocalizationSettings.StringDatabase.GetLocalizedString("VoiceCommandLocalization", "moregreen");
        moreBlueKey = LocalizationSettings.StringDatabase.GetLocalizedString("VoiceCommandLocalization", "moreblue");
        lessRedKey = LocalizationSettings.StringDatabase.GetLocalizedString("VoiceCommandLocalization", "lessred");
        lessGreenKey = LocalizationSettings.StringDatabase.GetLocalizedString("VoiceCommandLocalization", "lessgreen");
        lessBlueKey = LocalizationSettings.StringDatabase.GetLocalizedString("VoiceCommandLocalization", "lessblue");
    }

    private void Start()
    {
        // Get the reference to the instantiated prefab
        prefabInstance = prefabSelector.GetInstantiatedPrefab();

        // originalMaterial = new Material(Shader.Find("Standard"));
        originalMaterial = Instantiate(prefabInstance.GetComponent<Renderer>().material);
        // originalMaterial = new Material(Shader.Find("Transparent/Diffuse"));
        // originalMaterial = new Material(Shader.Find("Standard (Specular setup)"));
        originalMaterial.color = startingColor;

        // Set rendering mode to Transparent
        originalMaterial.SetFloat("_Mode", 3); // 3 corresponds to "Transparent" rendering mode
        originalMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        originalMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        originalMaterial.SetInt("_ZWrite", 0);
        originalMaterial.DisableKeyword("_ALPHATEST_ON");
        originalMaterial.EnableKeyword("_ALPHABLEND_ON");
        originalMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        originalMaterial.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

        // Use the material of the prefab instance
        prefabInstance.GetComponent<Renderer>().material = originalMaterial;

        currentColor = startingColor;  // Set the currentColor to the starting color

        keywordRecognizer = new KeywordRecognizer(new string[] { moreKey, lessKey, stopKey, redKey, greenKey, blueKey, moreRedKey, moreGreenKey, moreBlueKey, lessRedKey, lessGreenKey, lessBlueKey });
        keywordRecognizer.OnPhraseRecognized += RecognizedSpeech;

        // Get the ColorSoundManager script attached to this GameObject
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
            currentAxis = ' '; // Reset currentAxis to allow color to be selected in the next command

            // Reset the flag to allow audio playback
            stopByColorConditions = false;
        }
        else if (currentWord.StartsWith(moreKey) || currentWord.StartsWith(lessKey))
        {
            // Handle single keywords "morered," "lessred," etc.
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
            // Only allow "red", "green", or "blue" after "more" or "less"
            if (currentWord == redKey || currentWord == greenKey || currentWord == blueKey)
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

        // Indicate that "stop" was initiated by color conditions
        stopByColorConditions = true;

        // Require saying "stop" after "more" or "less" followed by the color
        lastCommand = stopKey;

        // Continue listening after the animation
        StartListening();

        // Stop all sounds using the new method
        colorSoundManager.StopAllSounds();
    }

    private IEnumerator ContinuousColorAnimation()
    {
        float increment = (lastCommand.StartsWith(moreKey)) ? 1.0f : -1.0f;

        // Reset the flag at the beginning of the animation
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

            // Only update the Albedo color of the material
            Color newAlbedoColor = new Color(red / 255f, green / 255f, blue / 255f, originalMaterial.color.a);
            prefabInstance.GetComponent<Renderer>().material.SetColor("_Color", newAlbedoColor);

            currentColor = new Color(red / 255f, green / 255f, blue / 255f);

            prefabInstance.GetComponent<Renderer>().material.color = currentColor;

            // Check if any color channel reaches 0 or 255 and stop the sound
            if ((red == 0 || red == 255 || green == 0 || green == 255 || blue == 0 || blue == 255) && !stopByColorConditions)
            {
                colorSoundManager.StopAllSounds();
                stopByColorConditions = false; // Set to false to continue listening after the animation
                lastCommand = stopKey;
                yield break; // Exit the coroutine early
            }

            // Check if the "stop" command is detected during the animation
            if (lastCommand == stopKey)
            {
                StopColorAnimation();
                yield break; // Exit the coroutine early
            }

            // Only yield if the color has changed
            if (previousRed != red || previousGreen != green || previousBlue != blue)
            {
                Debug.Log($"Increment Values: R = {red}, G = {green}, B = {blue}");
                yield return new WaitForSeconds(animationSpeed / 255f);
            }
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

        // Play the sound before starting the animation
        PlaySound();

        // Initialize color values based on currentColor
        red = Mathf.RoundToInt(currentColor.r * 255f);
        green = Mathf.RoundToInt(currentColor.g * 255f);
        blue = Mathf.RoundToInt(currentColor.b * 255f);

        // Start a new coroutine for continuous color animation
        colorAnimationCoroutine = StartCoroutine(ContinuousColorAnimation());
    }

    private void PlaySound()
    {
        // Play the corresponding sound based on color and direction
        if (!stopByColorConditions) // Only play sound if not stopped by color conditions
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

    private void StopListening()
    {
        if (isListening)
        {
            keywordRecognizer.Stop();
            isListening = false;
            Debug.Log("Stopped listening...");
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from the event when the script is destroyed
        LocalizationSettings.SelectedLocaleChanged -= OnSelectedLocaleChanged;
    }
}