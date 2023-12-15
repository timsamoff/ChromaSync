using UnityEngine;

public class PrefabInstantiator : MonoBehaviour
{
    [SerializeField]
    private GameObject prefabPrefab; // Serialized field for the prefab

    private GameObject instantiatedPrefab; // Reference to the instantiated prefab

    public void InstantiatePrefab()
    {
        if (instantiatedPrefab == null)
        {
            instantiatedPrefab = Instantiate(prefabPrefab, transform.position, Quaternion.identity, transform);
        }
    }

    public GameObject GetInstantiatedPrefab()
    {
        return instantiatedPrefab;
    }
}