using UnityEngine;

public class RandomPrefabSelector : MonoBehaviour
{
    [SerializeField]
    private GameObject[] prefabOptions; // Assign prefabs directly in the inspector

    private GameObject instantiatedPrefab; // Reference to the instantiated prefab
    private bool hasInstantiated = false; // Flag to track if instantiation has occurred

    [SerializeField]
    private ColorAnalyzer colorAnalyzer; // Reference to the ColorAnalyzer script

    void Start()
    {
        if (colorAnalyzer == null)
        {
            Debug.LogError("ColorAnalyzer script reference not set in the inspector.");
            return;
        }

        if (!hasInstantiated && prefabOptions != null && prefabOptions.Length > 0)
        {
            // Get a random index
            int randomIndex = Random.Range(0, prefabOptions.Length);

            // Instantiate the selected prefab
            instantiatedPrefab = Instantiate(prefabOptions[randomIndex], transform.position, Quaternion.identity);

            // Optionally, you can parent the instantiated prefab to the current GameObject
            instantiatedPrefab.transform.SetParent(transform);

            instantiatedPrefab.name = "object";

            Debug.Log("Instantiated Object name:" + " " + instantiatedPrefab.name);

            hasInstantiated = true; // Set the flag to true
        }
        else
        {
            Debug.LogWarning("Prefab already instantiated or no prefab options assigned.");
        }
    }

    public GameObject GetInstantiatedPrefab()
    {
        return instantiatedPrefab;
    }
}