using UnityEngine;

public class ColorChangeEvent : MonoBehaviour
{
    public delegate void ColorChangeEventHandler(Color newColor);
    public static event ColorChangeEventHandler OnColorChange;

    public static void TriggerColorChange(Color newColor)
    {
        if (OnColorChange != null)
            OnColorChange(newColor);
    }
}