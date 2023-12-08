using UnityEngine;

public class RandomBackgroundColor : MonoBehaviour
{
    private Color[] validColors;  // Precomputed list of valid colors

    private void Start()
    {
        // Precompute the list of valid colors during development
        PrecomputeValidColors();

        ChangeBackgroundColor();
    }

    private void PrecomputeValidColors()
    {
        // Example: Precompute 1000 valid colors
        int colorCount = 1000;
        validColors = new Color[colorCount];

        for (int i = 0; i < colorCount; i++)
        {
            validColors[i] = GetRandomColor();
            while (IsBlackOrGray(validColors[i]))
            {
                validColors[i] = GetRandomColor();
            }
        }
    }

    private void ChangeBackgroundColor()
    {
        // Select a random color from the precomputed list
        Color randomColor = validColors[Random.Range(0, validColors.Length)];

        // Apply the random color as the background color
        Camera.main.backgroundColor = randomColor;

        // Debug the RGB values as integers
        Debug.Log($"Background Color Changed to: R = {Mathf.RoundToInt(randomColor.r * 255f)}, " +
                  $"G = {Mathf.RoundToInt(randomColor.g * 255f)}, " +
                  $"B = {Mathf.RoundToInt(randomColor.b * 255f)}");
    }

    private Color GetRandomColor()
    {
        return new Color(Random.value, Random.value, Random.value);
    }

    private bool IsBlackOrGray(Color color)
    {
        // Check if the color is close to black or gray
        return color.r < 0.1f && color.g < 0.1f && color.b < 0.1f ||
               color.r > 0.8f && color.g > 0.8f && color.b > 0.8f;
    }
}