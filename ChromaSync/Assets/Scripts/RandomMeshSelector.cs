using UnityEngine;

public class RandomPrefabSelector : MonoBehaviour
{
    [SerializeField]
    private GameObject[] prefabOptions; // Assign prefabs directly in the inspector

    private GameObject instantiatedPrefab; // Reference to the instantiated prefab
    private bool hasInstantiated = false; // Flag to track if instantiation has occurred

    public void Start()
    {
        if (!hasInstantiated && prefabOptions != null && prefabOptions.Length > 0)
        {
            // Get a random index
            int randomIndex = Random.Range(0, prefabOptions.Length);

            // Instantiate the selected prefab
            instantiatedPrefab = Instantiate(prefabOptions[randomIndex], transform.position, Quaternion.identity);

            // Optionally, you can parent the instantiated prefab to the current GameObject
            instantiatedPrefab.transform.SetParent(transform);

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