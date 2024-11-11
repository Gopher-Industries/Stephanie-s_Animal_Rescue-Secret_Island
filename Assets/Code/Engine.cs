using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Engine : MonoBehaviour
{
    // List to store all loaded prefabs and models
    private List<GameObject> lstCubePrefabs = new List<GameObject>();

    // Folder path within the Assets folder
    private string prefabFolderPath = "Assets/Bitgem/Cube_World/Prefabs";

    void Start()
    {
        // Load all prefabs and models from the folder into lstCubePrefabs
        LoadAllAssets();

        // Specify the name of the prefab you're searching for
        string find = "ground_cube_grass";

        // Attempt to find the specified prefab in the list
        GameObject groundBlockPrefab = FindAssetByName(find);

        if (groundBlockPrefab != null)
        {
            // Create a 9x9 ground grid centered at (0, 0, 0)
            StrippedCollisionPrintGround(groundBlockPrefab, 500, 500, Vector3.zero);
        }
        else
        {
            Debug.LogError("Asset not found in list: " + find);
        }
    }

    private void LoadAllAssets()
    {
#if UNITY_EDITOR
        // Get all prefab files at the specified path
        string[] assetGuids = AssetDatabase.FindAssets("t:prefab", new[] { prefabFolderPath });

        if (assetGuids.Length == 0)
        {
            Debug.LogError($"No prefabs found at path: {prefabFolderPath}");
            Debug.LogWarning("Please ensure that the path is correct and contains prefabs.");
            return;
        }

        foreach (string guid in assetGuids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

            if (prefab != null)
            {
                lstCubePrefabs.Add(prefab);
                //Debug.Log("Loaded prefab: " + prefab.name);
            }
        }

        Debug.Log($"Loaded {lstCubePrefabs.Count} prefabs from {prefabFolderPath}");
#else
            Debug.LogError("This script requires the Unity Editor to load assets. For builds, please modify to use Resources or Addressables.");
#endif
    }

    private GameObject FindAssetByName(string assetName)
    {
        foreach (GameObject asset in lstCubePrefabs)
        {
            if (asset.name.Equals(assetName, System.StringComparison.OrdinalIgnoreCase))
            {
                //Debug.Log($"Asset '{assetName}' found.");
                return asset;
            }
        }
        Debug.LogWarning($"Asset not found: {assetName}");
        return null;
    }

    private void PrintGround(GameObject prefab, int gridWidth, int gridHeight, Vector3 centerPosition)
    {
        // Calculate the offset to start at the center
        Vector3 startOffset = new Vector3(-(gridWidth - 1) / 2.0f, 0, -(gridHeight - 1) / 2.0f);

        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                // Calculate the position for each instance based on the offset
                Vector3 position = centerPosition + startOffset + new Vector3(x, 0, z);

                // Instantiate the prefab at the calculated position
                GameObject instance = Instantiate(prefab, position, Quaternion.identity);
                //instance.isStatic = true;
                // Set name and parent for organization in the hierarchy
                instance.name = $"{prefab.name}_{x}_{z}";
                instance.transform.parent = this.transform;

                // Add a Box Collider for basic collision detection if not already present
                if (instance.GetComponent<BoxCollider>() == null)
                {
                    instance.AddComponent<BoxCollider>();
                }

                //Debug.Log($"Instantiated '{prefab.name}' at position {position} with name '{instance.name}'.");
            }
        }

        Debug.Log($"Successfully created a {gridWidth}x{gridHeight} grid of '{prefab.name}' instances.");
    }

    private void StrippedCollisionPrintGround(GameObject prefab, int gridWidth, int gridHeight, Vector3 centerPosition)
    {
        // Calculate the offset to start at the center
        Vector3 startOffset = new Vector3(-(gridWidth - 1) / 2.0f, 0, -(gridHeight - 1) / 2.0f);

        for (int x = 0; x < gridWidth; x++)
        {
            // Create a parent GameObject for each row
            GameObject rowParent = new GameObject($"Row_{x}");
            rowParent.transform.parent = this.transform;

            for (int z = 0; z < gridHeight; z++)
            {
                // Calculate the position for each instance based on the offset
                Vector3 position = centerPosition + startOffset + new Vector3(x, 0, z);

                // Instantiate the prefab at the calculated position
                GameObject instance = Instantiate(prefab, position, Quaternion.identity, rowParent.transform);

                // Set name for organization in the hierarchy
                instance.name = $"{prefab.name}_{x}_{z}";

                // Optionally, add a Box Collider if necessary (Removed to prevent individual colliders)
                // if (instance.GetComponent<BoxCollider>() == null)
                // {
                //     instance.AddComponent<BoxCollider>();
                // }

                // Optionally, disable rendering or physics for distant chunks
                // instance.SetActive(false); // Example: Disable for manual activation
            }

            // After instantiating all blocks in the row, add a single BoxCollider to the row parent
            BoxCollider rowCollider = rowParent.AddComponent<BoxCollider>();

            // Calculate the size of the collider based on the number of blocks
            // Assuming each block is 1 unit in size. Adjust if different.
            rowCollider.size = new Vector3(1, 1, gridHeight);
            rowCollider.center = new Vector3(0, 0, (gridHeight - 1) / 2.0f);

            // Optionally, mark the row as static for optimization
            rowParent.isStatic = true;

            // Optionally, adjust the layer or other properties as needed
            // rowParent.layer = LayerMask.NameToLayer("Ground");
        }

        Debug.Log($"Successfully created a {gridWidth}x{gridHeight} grid of '{prefab.name}' instances with stripped colliders.");
    }

}