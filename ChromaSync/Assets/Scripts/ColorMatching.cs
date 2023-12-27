using UnityEngine;

public class ColorMatching : MonoBehaviour
{
    public Material objectMaterial;
    public float difficultyLevel = 5f; // Default difficulty level

    [Header("Debug")]
    [SerializeField] private Color expectedColor; // Serialized for inspection purposes

    void Update()
    {
        // Set the expectedColor to the current background color
        expectedColor = Camera.main.backgroundColor;

        Color objectColor = objectMaterial.color;

        // Calculate differences for each channel
        float deltaR = Mathf.Abs(objectColor.r - expectedColor.r);
        float deltaG = Mathf.Abs(objectColor.g - expectedColor.g);
        float deltaB = Mathf.Abs(objectColor.b - expectedColor.b);

        // Calculate overall difference based on difficulty level
        float overallDifference = (deltaR + deltaG + deltaB) / 3f;

        // Adjusting for difficulty level
        float allowedDifference = 1f - difficultyLevel / 10f;

        if (overallDifference < allowedDifference)
        {
            // Matching success
            Debug.Log("Color matching success!");
        }
        else
        {
            // Not matched yet
            Debug.Log("Adjust the color to match the expected color.");
        }
    }
}