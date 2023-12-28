using UnityEngine;

public class AxialScanEffect : MonoBehaviour
{
    public float scanTime = 5.0f; // Total time to scan the entire object
    public Vector3 scanAxis = Vector3.up; // Change this to control the scan direction

    private Material material;
    private bool isScanning = false;
    private float startTime;

    void Start()
    {
        // Assuming the GameObject has a Renderer component with a material
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            material = renderer.material;
        }
        else
        {
            Debug.LogError("AxialScanEffect script requires a Renderer component with a material.");
            enabled = false; // Disable the script if the requirements are not met
        }
    }

    void Update()
    {
        if (isScanning && material != null)
        {
            // Calculate the scan speed dynamically based on the object's size
            float scanSpeed = 1.0f / scanTime;

            // Calculate the elapsed time since the scan started
            float elapsedScanTime = Time.time - startTime;

            // Update the scan offset based on time and speed
            float scanOffset = Mathf.Clamp01(scanSpeed * elapsedScanTime);

            // Pass the scan offset to the shader as a property
            material.SetFloat("_ScanOffset", scanOffset);
            material.SetVector("_ScanAxis", scanAxis);

            // Check if the entire object has been scanned, then stop the scanning effect
            if (scanOffset >= 1.0f)
            {
                StopScanEffect();
            }
        }

        // Check for the Space key to trigger the test effect
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TriggerScanEffect();
        }
    }

    // Call this method to trigger the scanning effect
    public void TriggerScanEffect()
    {
        isScanning = true;
        startTime = Time.time; // Record the start time when starting the scan

        // Set the _ObjectToWorld property in the shader
        material.SetMatrix("_ObjectToWorld", transform.localToWorldMatrix);

        Debug.Log("Scanning effect triggered.");
    }

    // You can call this method when you want to stop the scanning effect
    public void StopScanEffect()
    {
        isScanning = false;
        Debug.Log("Scanning effect stopped.");
    }
}