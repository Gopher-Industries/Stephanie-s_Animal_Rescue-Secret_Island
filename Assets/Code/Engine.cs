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

    // Direct reference to the ground block prefab
    [SerializeField] private GameObject groundBlockPrefab;

    // Optional parameters for grid size
    [SerializeField] private int gridWidth = 10;
    [SerializeField] private int gridHeight = 10;
    //[SerializeField] private Vector3 blockScale = Vector3.one; // Adjust based on your prefab's scale
    [SerializeField] private Vector3 blockScale = new Vector3(10, 1, 10); // Increased to make blocks 10x larger




    void Start()
    {
        // Load all prefabs and models from the folder into lstCubePrefabs
        //LoadAllAssets();

        // Specify the name of the prefab you're searching for
        string find = "ground_cube_grass";

        // Attempt to find the specified prefab in the list
        //GameObject groundBlockPrefab = FindAssetByName(find);

        if (groundBlockPrefab != null)
        {
            // Create a 9x9 ground grid centered at (0, 0, 0)
            //SingleColliderPrintGround(groundBlockPrefab, gridWidth, gridHeight, Vector3.zero);
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
        // Create a parent object to hold all rows
        GameObject groundParent = new GameObject("Ground_Parent");
        groundParent.transform.parent = this.transform;

        // Calculate the offset to start at the center
        Vector3 startOffset = new Vector3(-(gridWidth - 1) / 2.0f, 0, -(gridHeight - 1) / 2.0f);

        // Get the ground layer ID once
        int groundLayer = LayerMask.NameToLayer("Ground");

        // Get the block scale from the prefab to adjust collider size
        Vector3 blockScale = prefab.transform.localScale;

        for (int x = 0; x < gridWidth; x++)
        {
            // Create a parent GameObject for this row strip
            GameObject rowStrip = new GameObject($"Row_Strip_{x}");
            rowStrip.transform.parent = groundParent.transform;
            rowStrip.layer = groundLayer;

            // Position the strip at the start of this row
            rowStrip.transform.position = centerPosition + startOffset + new Vector3(x, 0, 0);

            // Create all blocks for this row
            for (int z = 0; z < gridHeight; z++)
            {
                // Calculate local position relative to the row strip
                Vector3 localPosition = new Vector3(0, 0, z);

                // Instantiate the block as a child of the row strip
                GameObject block = Instantiate(prefab, rowStrip.transform);
                block.transform.localPosition = localPosition;
                block.name = $"Block_{x}_{z}";
                block.layer = groundLayer;

                // Remove any existing colliders from the block
                Collider[] colliders = block.GetComponents<Collider>();
                foreach (Collider collider in colliders)
                {
                    Destroy(collider);
                }
            }

            // Add a single box collider for the entire row
            BoxCollider stripCollider = rowStrip.AddComponent<BoxCollider>();

            // Set the collider size to cover the entire row
            // Width is block scale, height is block scale, length is total row length
            stripCollider.size = new Vector3(blockScale.x, blockScale.y, gridHeight * blockScale.z);

            // Center the collider along the row
            stripCollider.center = new Vector3(0, 0, (gridHeight - 1) * blockScale.z / 2f);

            // Mark the strip as static for physics optimization
            rowStrip.isStatic = true;
        }

        Debug.Log($"Created ground grid: {gridWidth}x{gridHeight} blocks with {gridWidth} collider strips");
    }

    private void SingleColliderPrintGround(GameObject prefab, int gridWidth, int gridHeight, Vector3 centerPosition)
    {
        // Create a single parent object that will hold everything including the one collider
        GameObject groundParent = new GameObject("Ground_Parent_Single_Collider");
        groundParent.transform.parent = this.transform;
        groundParent.layer = LayerMask.NameToLayer("Ground");

        // Calculate the total size of the grid
        Vector3 totalSize = new Vector3(gridWidth * blockScale.x, blockScale.y, gridHeight * blockScale.z);

        // Calculate the offset to start at the center
        // This ensures the grid is centered around 'centerPosition'
        Vector3 startOffset = new Vector3(
            -(totalSize.x / 2f) + (blockScale.x / 2f),
            0,
            -(totalSize.z / 2f) + (blockScale.z / 2f)
        );

        // Position the parent at the center position
        groundParent.transform.position = centerPosition;

        // Create a container for all the visual blocks
        GameObject blocksContainer = new GameObject("Blocks_Container");
        blocksContainer.transform.parent = groundParent.transform;
        blocksContainer.transform.localPosition = Vector3.zero;
        blocksContainer.layer = LayerMask.NameToLayer("Ground");

        // Create all the visual blocks
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                // Calculate world position for each block
                Vector3 position = startOffset + new Vector3(x * blockScale.x, 0, z * blockScale.z);

                // Instantiate the block as a child of the blocks container
                GameObject block = Instantiate(prefab, position, Quaternion.identity, blocksContainer.transform);
                block.transform.localScale = blockScale; // Apply the block scale
                block.name = $"Block_{x}_{z}";
                block.layer = LayerMask.NameToLayer("Ground");

                // Remove any existing colliders from the block for optimization
                Collider[] colliders = block.GetComponents<Collider>();
                foreach (Collider collider in colliders)
                {
                    Destroy(collider);
                }
            }
        }

        // Add a single BoxCollider to the parent that covers the entire grid
        BoxCollider gridCollider = groundParent.AddComponent<BoxCollider>();

        // Set the collider size based on the total grid size
        gridCollider.size = totalSize;

        // Position the collider at the center of the grid
        gridCollider.center = Vector3.zero;

        // Optionally, mark as static for physics optimization
        groundParent.isStatic = true;

        Debug.Log($"Created {gridWidth}x{gridHeight} ground grid with a single collider of size {gridCollider.size}");
    }


    /// <summary>
    /// Combines all ground meshes into a single mesh.
    /// </summary>
    private void CombineGroundMeshes()
    {
        // Find the Blocks_Container GameObject
        GameObject blocksContainer = transform.Find("Ground_Parent_Single_Collider/Blocks_Container")?.gameObject;
        if (blocksContainer == null)
        {
            Debug.LogError("Blocks_Container not found. Ensure that SingleColliderPrintGround has been called.");
            return;
        }

        // Get all MeshFilters in the blocks container
        MeshFilter[] meshFilters = blocksContainer.GetComponentsInChildren<MeshFilter>();
        if (meshFilters.Length == 0)
        {
            Debug.LogWarning("No MeshFilters found in Blocks_Container.");
            return;
        }

        // Prepare CombineInstance array
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
        }

        // Create a new GameObject to hold the combined mesh
        GameObject combinedMeshObject = new GameObject("CombinedGroundMesh");
        combinedMeshObject.transform.parent = transform;
        combinedMeshObject.layer = LayerMask.NameToLayer("Ground");

        // Add MeshFilter and MeshRenderer
        MeshFilter mf = combinedMeshObject.AddComponent<MeshFilter>();
        MeshRenderer mr = combinedMeshObject.AddComponent<MeshRenderer>();

        // Assign the combined mesh
        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combine, true, true);
        mf.mesh = combinedMesh;

        // Assign a material (assuming the prefab has one)
        MeshRenderer originalRenderer = meshFilters[0].GetComponent<MeshRenderer>();
        if (originalRenderer != null)
        {
            mr.sharedMaterial = originalRenderer.sharedMaterial;
        }
        else
        {
            mr.sharedMaterial = new Material(Shader.Find("Standard"));
            Debug.LogWarning("Original MeshRenderer not found. Assigned default material.");
        }

        // Optionally, mark the combined mesh as static
        combinedMeshObject.isStatic = true;

        // Optionally, destroy the original blocks
        foreach (MeshFilter mfChild in meshFilters)
        {
            Destroy(mfChild.gameObject);
        }

        Debug.Log($"Combined {meshFilters.Length} ground meshes into a single mesh.");
    }




}