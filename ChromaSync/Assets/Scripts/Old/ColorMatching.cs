using UnityEngine;
using UnityEngine.SceneManagement;

public class ColorMatching : MonoBehaviour
{
    public Material objectMaterial;
    public float difficultyLevel = 4f; // Default difficulty level
    public float delayBeforeRestart = 5f; // Delay in seconds before restarting the scene

    private bool isMatching = false;
    private float matchTime = 0f;

    [Header("Debug")]
    [SerializeField] private Color expectedColor; // Serialized for inspection purposes

    void Update()
    {
        // Set the expectedColor to the current background color
        expectedColor = Camera.main.backgroundColor;

        // Get the color of the object material
        Color objectColor = objectMaterial.color;

        // Convert RGB to HSL
        float[] expectedHSL = RGBToHSL(expectedColor);
        float[] objectHSL = RGBToHSL(objectColor);

        // Calculate hue differences
        float deltaHue = Mathf.Abs(expectedHSL[0] - objectHSL[0]);

        // Adjusting for difficulty level using an exponential function
        float difficultyScale = Mathf.Pow(2f, difficultyLevel - 1f);
        float allowedDifference = 360f - difficultyScale * 36f; // 36 is an arbitrary value, adjust as needed

        if (deltaHue < allowedDifference)
        {
            // Matching success
            if (!isMatching)
            {
                isMatching = true;
                matchTime = Time.time;
                Debug.Log("Color matching success! Waiting for " + delayBeforeRestart + " seconds.");
            }

            // Check if the delay time has passed
            if (isMatching && Time.time - matchTime >= delayBeforeRestart)
            {
                // Restart the scene
                Debug.Log("Restarting the scene.");
                RestartScene();
            }
        }
        else
        {
            // Not matched or no longer matched
            isMatching = false;
            Debug.Log("Adjust the color to match the expected color.");
        }
    }

    void RestartScene()
    {
        // Reload the current scene
        Scene currentScene = SceneManager.GetActiveScene();
        int sceneIndex = currentScene.buildIndex;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Single);

        // Check for errors
        if (asyncLoad == null)
        {
            Debug.LogError("Failed to load the scene.");
        }
    }

    // Helper method to convert RGB to HSL
    private float[] RGBToHSL(Color color)
    {
        float r = color.r;
        float g = color.g;
        float b = color.b;

        float max = Mathf.Max(r, Mathf.Max(g, b));
        float min = Mathf.Min(r, Mathf.Min(g, b));

        float h = 0f;
        float s = 0f;
        float l = (max + min) / 2f;

        if (max != min)
        {
            float d = max - min;
            s = l > 0.5f ? d / (2f - max - min) : d / (max + min);

            if (max == r)
                h = (g - b) / d + (g < b ? 6f : 0f);
            else if (max == g)
                h = (b - r) / d + 2f;
            else if (max == b)
                h = (r - g) / d + 4f;

            h /= 6f;
        }

        float[] hsl = { h * 360f, s * 100f, l * 100f };
        return hsl;
    }
}