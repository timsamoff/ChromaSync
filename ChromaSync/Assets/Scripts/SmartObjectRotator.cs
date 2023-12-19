using UnityEngine;
using System.Collections;

public class SmartObjectRotator : MonoBehaviour
{
    public enum TransitionType
    {
        Linear,
        ExponentialLogarithmic
    }

    [Header("Rotation Settings")]
    [SerializeField]
    private float rotationSpeed = 100f;

    [SerializeField]
    private float transitionDuration = 1f;

    [SerializeField]
    private float destinationDuration = 1f;

    [Header("Transition Type")]
    [SerializeField]
    private TransitionType transitionType = TransitionType.Linear;

    [Header("Exponential Logarithmic Settings")]
    [SerializeField]
    private float logarithmicRolloff = 1.0f;

    // Define ranges for each axis
    [Header("Rotation Ranges (-1.0 - 1.0)")]
    [SerializeField]
    private Vector2 xRange = new Vector2(-1.0f, 1.0f);

    [SerializeField]
    private Vector2 yRange = new Vector2(-1.0f, 1.0f);

    [SerializeField]
    private Vector2 zRange = new Vector2(-1.0f, 1.0f);

<<<<<<< HEAD
    [Header("Rotation Values")]
=======
    [Header("Rotation Values (Offset defaults for starting rotation)")]
>>>>>>> 18dc7545238d97c328e3be7bfb9ad0035805e2be
    [SerializeField, Range(-1.0f, 1.0f)]
    private float xRotation = 0.0f;

    [SerializeField, Range(-1.0f, 1.0f)]
    private float yRotation = 0.0f;

    [SerializeField, Range(-1.0f, 1.0f)]
    private float zRotation = 0.0f;

    private void Start()
    {
        // Set the initial rotation to a completely random angle
        xRotation = GetClampedRandomRotation(xRange);
        yRotation = GetClampedRandomRotation(yRange);
        zRotation = GetClampedRandomRotation(zRange);

        // Rotate the object to the random initial rotation
        transform.Rotate(new Vector3(xRotation, yRotation, zRotation) * rotationSpeed);

        // Start the coroutine for continuous rotation
        StartCoroutine(ChangeRotation());
    }

    private IEnumerator TransitionToInitialRotation(Quaternion initialRotation)
    {
        float elapsedTime = 0f;
        float transitionTime = 2f; // You can adjust the duration of the transition

        while (elapsedTime < transitionTime)
        {
            float t = elapsedTime / transitionTime;
            transform.rotation = Quaternion.Slerp(initialRotation, Quaternion.Euler(new Vector3(xRotation, yRotation, zRotation) * rotationSpeed), t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final rotation exactly matches the random initial rotation
        transform.rotation = Quaternion.Euler(new Vector3(xRotation, yRotation, zRotation) * rotationSpeed);

        // Start the coroutine for continuous rotation
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
                transform.Rotate(new Vector3(xRotation, yRotation, zRotation) * rotationSpeed * Time.deltaTime);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Transition to new destination values using transitionDuration
            float targetXRot = RoundRotation(GetClampedRandomRotation(xRange));
            float targetYRot = RoundRotation(GetClampedRandomRotation(yRange));
            float targetZRot = RoundRotation(GetClampedRandomRotation(zRange));

            float initialXRot = xRotation;
            float initialYRot = yRotation;
            float initialZRot = zRotation;

            elapsedTime = 0f; // Reset the time for the transition to the next clamped value

            // Transition to the new destination values
            while (elapsedTime < transitionDuration)
            {
                float t = elapsedTime / transitionDuration;

                if (transitionType == TransitionType.Linear)
                {
                    xRotation = Mathf.Lerp(initialXRot, targetXRot, t);
                    yRotation = Mathf.Lerp(initialYRot, targetYRot, t);
                    zRotation = Mathf.Lerp(initialZRot, targetZRot, t);
                }
                else if (transitionType == TransitionType.ExponentialLogarithmic)
                {
                    xRotation = Mathf.Lerp(initialXRot, targetXRot, Mathf.Pow(t, logarithmicRolloff));
                    yRotation = Mathf.Lerp(initialYRot, targetYRot, Mathf.Pow(t, logarithmicRolloff));
                    zRotation = Mathf.Lerp(initialZRot, targetZRot, Mathf.Pow(t, logarithmicRolloff));
                }

                // Rotate the object using rotationSpeed
                transform.Rotate(new Vector3(xRotation, yRotation, zRotation) * rotationSpeed * Time.deltaTime);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Match the target values
            xRotation = targetXRot;
            yRotation = targetYRot;
            zRotation = targetZRot;

            yield return null; // Delay before choosing the next random rotation
        }
    }

    private float GetClampedRandomRotation(Vector2 range)
    {
        float[] possibleValues = { -1.0f, -0.9f, -0.8f, -0.7f, -0.6f, -0.5f, -0.4f, -0.3f, -0.2f, -0.1f, 0.0f, 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f, 1.0f };
        return Mathf.Clamp(possibleValues[Random.Range(0, possibleValues.Length)], range.x, range.y);
    }

    private float RoundRotation(float value)
    {
        return Mathf.Round(value * 10f) / 10f;
    }
}