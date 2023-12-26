using UnityEngine;

public class ColorAnalyzer : MonoBehaviour
{
    [Header("Difficulty Level")]
    [Tooltip("Set the difficulty level from 1 to 10, where 1 is the most lenient and 10 is the most strict.")]
    [SerializeField, Range(1, 10)] private int difficultyLevel = 5;

    void Update()
    {
        // Get the background color and convert to HSV
        Color backgroundColor = Camera.main.backgroundColor;
        float[] backgroundHSV = new float[3];
        Color.RGBToHSV(backgroundColor, out backgroundHSV[0], out backgroundHSV[1], out backgroundHSV[2]);

        // Get the material color and convert to HSV
        Renderer renderer = GetComponent<Renderer>();
        Material material = renderer.material;
        Color materialColor = material.color;
        float[] materialHSV = new float[3];
        Color.RGBToHSV(materialColor, out materialHSV[0], out materialHSV[1], out materialHSV[2]);

        // Calculate HSV thresholds based on difficulty level
        float thresholdMultiplier = 1.1f - difficultyLevel * 0.1f; // Convert 1-10 to 1.0-0.1 range
        float hueThreshold = 0.1f * thresholdMultiplier;
        float saturationThreshold = 0.1f * thresholdMultiplier;
        float valueThreshold = 0.1f * thresholdMultiplier;

        // Compare HSV values
        bool colorsInRange =
            Mathf.Abs(backgroundHSV[0] - materialHSV[0]) < hueThreshold &&
            Mathf.Abs(backgroundHSV[1] - materialHSV[1]) < saturationThreshold &&
            Mathf.Abs(backgroundHSV[2] - materialHSV[2]) < valueThreshold;

        // Debug messages
        if (colorsInRange)
        {
            Debug.Log("Color matching complete!");
        }
        else
        {
            Debug.Log("Color out of range.");
        }

        // Adjust difficulty based on the color comparison
        if (colorsInRange)
        {
            // Colors are within the specified range (easier)
            AdjustDifficulty(1.0f);
        }
        else
        {
            // Colors are not within the specified range (harder)
            AdjustDifficulty(0.5f);
        }
    }

    void AdjustDifficulty(float modifier)
    {
        // Add your difficulty adjustment logic here
        // For example, you might adjust the speed or health of the object
        // based on the modifier value.
    }
}