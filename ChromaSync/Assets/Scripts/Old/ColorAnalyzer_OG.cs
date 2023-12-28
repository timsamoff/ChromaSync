using UnityEngine;

public class ColorAnalyzer_OG : MonoBehaviour
{
    [Header("Difficulty Level")]
    [Tooltip("Set the difficulty level from 1 to 10, where 1 is the most lenient and 10 is the most strict.")]
    [SerializeField, Range(1, 10)] private int difficultyLevel = 5;

    void Update()
    {
        // Get the background color and convert to Lab
        Color backgroundColor = Camera.main.backgroundColor;
        Vector3 backgroundLab = RGBtoLab(backgroundColor);

        // Get the material color and convert to Lab
        Renderer renderer = GetComponent<Renderer>();
        Material material = renderer.material;
        Color materialColor = material.color;
        Vector3 materialLab = RGBtoLab(materialColor);

        // Calculate Lab threshold based on difficulty level
        float thresholdMultiplier = 1.1f - difficultyLevel * 0.1f; // Convert 1-10 to 1.0-0.1 range
        float labThreshold = 10.0f * thresholdMultiplier; // Adjust the threshold based on your preferences

        // Compare Lab values
        bool labsInRange = Vector3.Distance(backgroundLab, materialLab) < labThreshold;

        // Debug messages
        if (labsInRange)
        {
            Debug.Log("Color matching complete!");
        }
        else
        {
            // Debug.Log("Color out of range.");
        }

        // Adjust difficulty based on the Lab comparison
        if (labsInRange)
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

    Vector3 RGBtoLab(Color color)
    {
        // Convert RGB to XYZ
        float r = color.r;
        float g = color.g;
        float b = color.b;

        r = (r > 0.04045f) ? Mathf.Pow((r + 0.055f) / 1.055f, 2.4f) : r / 12.92f;
        g = (g > 0.04045f) ? Mathf.Pow((g + 0.055f) / 1.055f, 2.4f) : g / 12.92f;
        b = (b > 0.04045f) ? Mathf.Pow((b + 0.055f) / 1.055f, 2.4f) : b / 12.92f;

        r *= 100.0f;
        g *= 100.0f;
        b *= 100.0f;

        float x = r * 0.4124564f + g * 0.3575761f + b * 0.1804375f;
        float y = r * 0.2126729f + g * 0.7151522f + b * 0.0721750f;
        float z = r * 0.0193339f + g * 0.1191920f + b * 0.9503041f;

        // Convert XYZ to Lab
        x = (x > 0.008856f) ? Mathf.Pow(x, 1.0f / 3.0f) : (903.3f * x + 16.0f) / 116.0f;
        y = (y > 0.008856f) ? Mathf.Pow(y, 1.0f / 3.0f) : (903.3f * y + 16.0f) / 116.0f;
        z = (z > 0.008856f) ? Mathf.Pow(z, 1.0f / 3.0f) : (903.3f * z + 16.0f) / 116.0f;

        float l = Mathf.Max(0.0f, 116.0f * y - 16.0f);
        float aLab = (x - y) * 500.0f;
        float bLab = (y - z) * 200.0f;

        return new Vector3(l, aLab, bLab);
    }
}