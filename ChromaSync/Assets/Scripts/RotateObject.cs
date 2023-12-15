using UnityEngine;
using System.Collections;

public class RotateObject : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField]
    private float rotationSpeed = 100f;

    [SerializeField]
    private float changeRotationInterval = 2f;

    [SerializeField]
    private float destinationDuration = 1f;

    [Header("Rotation Values")]
    [SerializeField]
    private float xRot = 0.5f;

    [SerializeField]
    private float yRot = 1.0f;

    [SerializeField]
    private float zRot = 0.5f;

    private void Start()
    {
        StartCoroutine(ChangeRotation());
    }

    private IEnumerator ChangeRotation()
    {
        while (true)
        {
            // Keep the current values for destinationDuration
            float elapsedTime = 0f;

            while (elapsedTime < destinationDuration)
            {
                // Rotate the object using rotationSpeed
                transform.Rotate(new Vector3(xRot, yRot, zRot) * rotationSpeed * Time.deltaTime);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Now, transition to new destination values using changeRotationInterval
            float targetXRot = RoundRotation(GetClampedRandomRotation());
            float targetYRot = RoundRotation(GetClampedRandomRotation());
            float targetZRot = RoundRotation(GetClampedRandomRotation());

            float initialXRot = xRot;
            float initialYRot = yRot;
            float initialZRot = zRot;

            elapsedTime = 0f; // Reset the time for the transition to the next clamped value

            // Smoothly transition to the new destination values
            while (elapsedTime < changeRotationInterval)
            {
                xRot = Mathf.Lerp(initialXRot, targetXRot, elapsedTime / changeRotationInterval);
                yRot = Mathf.Lerp(initialYRot, targetYRot, elapsedTime / changeRotationInterval);
                zRot = Mathf.Lerp(initialZRot, targetZRot, elapsedTime / changeRotationInterval);

                // Rotate the object using rotationSpeed
                transform.Rotate(new Vector3(xRot, yRot, zRot) * rotationSpeed * Time.deltaTime);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure final values match the target exactly
            xRot = targetXRot;
            yRot = targetYRot;
            zRot = targetZRot;

            yield return null; // Add a small delay before picking the next random rotation
        }
    }

    private float GetClampedRandomRotation()
    {
        float[] possibleValues = { 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f, 1.0f };
        return possibleValues[Random.Range(0, possibleValues.Length)];
    }

    private float RoundRotation(float value)
    {
        return Mathf.Round(value * 10f) / 10f;
    }
}