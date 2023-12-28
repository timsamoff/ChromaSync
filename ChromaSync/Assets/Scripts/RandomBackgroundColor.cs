using UnityEngine;

public class RandomBackgroundColor : MonoBehaviour
{
    // Event for background color changes
    public delegate void BackgroundColorChanged(Color newColor);
    public static event BackgroundColorChanged OnBackgroundColorChanged;

    private Color[] validColors;  // Precomputed list of valid colors

    // Property to get the current background color
    public static Color CurrentBackgroundColor { get; private set; }

    void Start()
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

    public void ChangeBackgroundColor()
    {
        // Select a random color from the precomputed list
        Color newColor = validColors[Random.Range(0, validColors.Length)];

        // Notify subscribers (e.g., ColorAnalyzer) that the background color has changed
        OnBackgroundColorChanged?.Invoke(newColor);

        // Update the current background color property
        CurrentBackgroundColor = newColor;

        // Apply the random color as the background color
        Camera.main.backgroundColor = newColor;

        Debug.Log("Background Color Changed to: R = " + (int)(newColor.r * 255) + ", G = " + (int)(newColor.g * 255) + ", B = " + (int)(newColor.b * 255));
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