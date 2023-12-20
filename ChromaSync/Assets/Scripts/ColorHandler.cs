/* using System.Collections;
using UnityEngine;

public class ColorHandler : MonoBehaviour
{
    [SerializeField] private Color startingColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

    public void SetColor(GameObject prefabInstance, Material originalMaterial, ref Color currentColor)
    {
        originalMaterial.color = startingColor;
        Debug.Log($"Material Color with Alpha: {originalMaterial.color}");
        prefabInstance.GetComponent<Renderer>().material = originalMaterial;
        currentColor = startingColor;
    }

    public IEnumerator ContinuousColorAnimation(
        MonoBehaviour monoBehaviour,
        ref string lastCommand,
        Material originalMaterial,
        GameObject prefabInstance,
        ref Color currentColor,
        float animationSpeed,
        ref int red,
        ref int green,
        ref int blue,
        ColorSoundManager colorSoundManager)
    {
        float increment = (lastCommand.StartsWith("more")) ? 1.0f : -1.0f;

        while (lastCommand.StartsWith("more") || lastCommand.StartsWith("less"))
        {
            float incrementValue = increment / 255f;
            int previousRed = red;
            int previousGreen = green;
            int previousBlue = blue;

            switch (lastCommand)
            {
                case "morered":
                    red += Mathf.RoundToInt(incrementValue * 255f);
                    red = Mathf.Clamp(red, 0, 255);
                    break;
                case "moregreen":
                    green += Mathf.RoundToInt(incrementValue * 255f);
                    green = Mathf.Clamp(green, 0, 255);
                    break;
                case "moreblue":
                    blue += Mathf.RoundToInt(incrementValue * 255f);
                    blue = Mathf.Clamp(blue, 0, 255);
                    break;
                case "lessred":
                    red -= Mathf.RoundToInt(incrementValue * 255f);
                    red = Mathf.Clamp(red, 0, 255);
                    break;
                case "lessgreen":
                    green -= Mathf.RoundToInt(incrementValue * 255f);
                    green = Mathf.Clamp(green, 0, 255);
                    break;
                case "lessblue":
                    blue -= Mathf.RoundToInt(incrementValue * 255f);
                    blue = Mathf.Clamp(blue, 0, 255);
                    break;
            }

            Color newAlbedoColor = new Color(red / 255f, green / 255f, blue / 255f, originalMaterial.color.a);
            Debug.Log($"Setting color to: {newAlbedoColor}");
            prefabInstance.GetComponent<Renderer>().material.color = newAlbedoColor;

            currentColor = new Color(red / 255f, green / 255f, blue / 255f);

            prefabInstance.GetComponent<Renderer>().material.color = currentColor;

            if ((red == 0 || red == 255 || green == 0 || green == 255 || blue == 0 || blue == 255))
            {
                colorSoundManager.StopAllSounds();
                yield return monoBehaviour.StartCoroutine(StopAnimation(colorSoundManager, ref lastCommand));
                yield break;
            }

            if (lastCommand == "stop")
            {
                yield return monoBehaviour.StartCoroutine(StopAnimation(colorSoundManager, ref lastCommand));
                yield break;
            }

            if (previousRed != red || previousGreen != green || previousBlue != blue)
            {
                Debug.Log($"Increment Values: R = {red}, G = {green}, B = {blue}");
                yield return new WaitForSeconds(animationSpeed / 255f);
            }
        }

        StopListening(monoBehaviour);
    }

    private IEnumerator StopAnimation(ColorSoundManager colorSoundManager, ref string lastCommand)
    {
        colorSoundManager.StopAllSounds();
        yield return new WaitForSeconds(0.1f); // Adjust delay if needed
        lastCommand = "stop";
    }

    private void StopListening(MonoBehaviour monoBehaviour)
    {
        // Placeholder: Implement logic to stop listening
        Debug.Log("Stopping listening...");
    }
} */