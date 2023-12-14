using UnityEngine;

public class RandomMeshSelector : MonoBehaviour
{
    public Mesh[] meshOptions;

    void Start()
    {
        if (meshOptions.Length > 0)
        {
            // Get a random index
            int randomIndex = Random.Range(0, meshOptions.Length);

            // Set the selected mesh
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                meshFilter.mesh = meshOptions[randomIndex];
            }
            else
            {
                Debug.LogError("MeshFilter component not found on the object.");
            }
        }
        else
        {
            Debug.LogError("No mesh options assigned to the array.");
        }
    }
}