using UnityEngine;

public class ColorComparator : MonoBehaviour
{
    [Range(1, 10)] // To restrict the range of difficultyLevel in the Unity Inspector
    public float difficultyLevel = 1f; // Serialized difficulty level

    private Renderer objectRenderer;
    private Camera mainCamera;

    void Start()
    {
        // Assuming your object and background have MeshRenderer components
        objectRenderer = GetComponent<Renderer>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Get the color of the object's material and the background
        Color objectColor = objectRenderer.material.color;
        Color backgroundColor = mainCamera.backgroundColor;

        // Check if the colors are visually similar based on the difficulty level
        bool colorsAreSimilar = AreColorsSimilar(objectColor, backgroundColor, difficultyLevel);

        if (colorsAreSimilar)
        {
            Debug.Log("Colors are visually similar!");
            // Add your logic for when colors are similar

            // You might want to stop updating the color or perform other actions here
        }
        else
        {
            Debug.Log("Colors are not visually similar.");
            // Add your logic for when colors are not similar

            // You might want to continue updating the color or perform other actions here
        }
    }

    bool AreColorsSimilar(Color color1, Color color2, float difficultyLevel)
    {
        // Debugging: Print out values
        Debug.Log($"Color Difference (R): {Mathf.Abs(color1.r - color2.r)}");
        Debug.Log($"Color Difference (G): {Mathf.Abs(color1.g - color2.g)}");
        Debug.Log($"Color Difference (B): {Mathf.Abs(color1.b - color2.b)}");

        // Check if each color channel is within a fixed range (adjust as needed)
        return IsWithinRange(color1.r, color2.r, difficultyLevel)
            && IsWithinRange(color1.g, color2.g, difficultyLevel)
            && IsWithinRange(color1.b, color2.b, difficultyLevel);
    }

    bool IsWithinRange(float value1, float value2, float difficultyLevel)
    {
        // Use a fixed range based on the difficulty level
        float fixedRange = Mathf.Lerp(20f, 5f, (difficultyLevel - 1) / 9f);

        // Check if the absolute difference between values is within the fixed range
        return Mathf.Abs(value1 - value2) <= fixedRange;
    }
}