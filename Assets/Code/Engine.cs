using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour
{
    // List to store all loaded prefabs
    private List<GameObject> lstCubePrefabs = new List<GameObject>();

    // Folder path within the Resources folder
    private string prefabFolderPath = "Bitgem/Cube_World/Prefabs";

    void Start()
    {
        // Load all prefabs from the folder into lstCubePrefabs
        LoadAllPrefabs();

        // Specify the name of the prefab you're searching for
        string find = "ground_cube_grass";

        // Attempt to find the specified prefab in the list
        GameObject canopyBlockPrefab = FindPrefabByName(find);

        if (canopyBlockPrefab != null)
        {
            // Create a 9x9 ground grid centered at (0, 0, 0)
            PrintGround(canopyBlockPrefab, 9, 9, new Vector3(0, 0, 0));
        }
        else
        {
            Debug.LogError("Prefab not found in list: " + find);
        }
    }

    // Load all prefabs in the specified folder and add them to lstCubePrefabs
    private void LoadAllPrefabs()
    {
        GameObject[] prefabs = Resources.LoadAll<GameObject>(prefabFolderPath);
        lstCubePrefabs.AddRange(prefabs);

        Debug.Log("Loaded " + lstCubePrefabs.Count + " prefabs from " + prefabFolderPath);

        // Debug each prefab name to confirm they are loaded correctly
        foreach (var prefab in lstCubePrefabs)
        {
            Debug.Log("Loaded prefab: " + prefab.name);
        }
    }

    // Find a prefab in lstCubePrefabs by its name
    private GameObject FindPrefabByName(string prefabName)
    {
        foreach (GameObject prefab in lstCubePrefabs)
        {
            if (prefab.name == prefabName)
            {
                return prefab;
            }
        }
        Debug.LogWarning("Prefab not found: " + prefabName);
        return null;
    }

    // Instantiate a centered grid of the specified prefab
    private void PrintGround(GameObject prefab, int gridWidth, int gridHeight, Vector3 centerPosition)
    {
        // Calculate the offset to start at the center
        Vector3 startOffset = new Vector3(-(gridWidth - 1) / 2.0f, 0, -(gridHeight - 1) / 2.0f);

        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                // Calculate the position for each prefab instance based on the offset
                Vector3 position = centerPosition + startOffset + new Vector3(x, 0, z);

                // Instantiate prefab at the calculated position
                GameObject instance = Instantiate(prefab, position, Quaternion.identity);

                // Set name and parent for organization in the hierarchy
                instance.name = $"{prefab.name}_{x}_{z}";
                instance.transform.parent = transform;

                // Add a Box Collider for basic collision detection if not already present
                if (instance.GetComponent<BoxCollider>() == null)
                {
                    instance.AddComponent<BoxCollider>();
                }
            }
        }
    }
}
