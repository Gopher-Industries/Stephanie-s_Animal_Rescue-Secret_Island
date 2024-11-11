using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Responsible for procedurally generating a ground grid in chunks, combining meshes, and adding a large BoxCollider.
/// </summary>
public class GroundGenerator
{
    // Configuration Parameters
    private GameObject groundBlockPrefab;
    private int gridWidth;
    private int gridHeight;
    private Vector3 blockScale;
    private Material groundMaterial;
    private string groundLayerName;
    private int chunkSize;

    // Parent GameObject to hold all ground chunks
    private GameObject chunksParent;

    /// <summary>
    /// Initializes a new instance of the GroundGenerator class.
    /// </summary>
    /// <param name="groundBlockPrefab">Prefab for each ground block.</param>
    /// <param name="gridWidth">Number of blocks along the X-axis.</param>
    /// <param name="gridHeight">Number of blocks along the Z-axis.</param>
    /// <param name="blockScale">Scale of each ground block.</param>
    /// <param name="groundMaterial">Material to apply to the combined mesh.</param>
    /// <param name="groundLayerName">Layer name for ground objects.</param>
    /// <param name="chunkSize">Number of tiles per chunk (default is 64).</param>
    public GroundGenerator(GameObject groundBlockPrefab, int gridWidth, int gridHeight, Vector3 blockScale, Material groundMaterial, string groundLayerName, int chunkSize = 64)
    {
        this.groundBlockPrefab = groundBlockPrefab;
        this.gridWidth = gridWidth;
        this.gridHeight = gridHeight;
        this.blockScale = blockScale;
        this.groundMaterial = groundMaterial;
        this.groundLayerName = groundLayerName;
        this.chunkSize = chunkSize;
    }

    /// <summary>
    /// Generates the ground grid by creating chunks, combining meshes, and adding a single BoxCollider.
    /// </summary>
    /// <param name="parent">Parent Transform under which the ground will be created.</param>
    public void GenerateGround(Transform parent)
    {
        if (groundBlockPrefab == null)
        {
            Debug.LogError("Ground Block Prefab is not assigned.");
            return;
        }

        // Step 1: Generate Ground Chunks
        CreateGroundChunks(parent);

        // Step 2: Add Single Large BoxCollider
        AddSingleBoxCollider(parent);
    }

    /// <summary>
    /// Creates ground chunks and combines meshes within each chunk.
    /// </summary>
    /// <param name="parent">Parent Transform.</param>
    private void CreateGroundChunks(Transform parent)
    {
        // Create a parent container for all chunks
        chunksParent = new GameObject("Chunks_Parent");
        chunksParent.transform.parent = parent;
        chunksParent.layer = LayerMask.NameToLayer(groundLayerName);

        int chunksX = Mathf.CeilToInt((float)gridWidth / chunkSize);
        int chunksZ = Mathf.CeilToInt((float)gridHeight / chunkSize);

        for (int cx = 0; cx < chunksX; cx++)
        {
            for (int cz = 0; cz < chunksZ; cz++)
            {
                int currentChunkWidth = (cx == chunksX - 1) ? gridWidth - cx * chunkSize : chunkSize;
                int currentChunkHeight = (cz == chunksZ - 1) ? gridHeight - cz * chunkSize : chunkSize;

                GameObject chunk = new GameObject($"Chunk_{cx}_{cz}");
                chunk.transform.parent = chunksParent.transform;
                chunk.layer = LayerMask.NameToLayer(groundLayerName);

                // Calculate the starting offset for this chunk
                Vector3 chunkStartOffset = new Vector3(
                    cx * chunkSize * blockScale.x - (gridWidth * blockScale.x) / 2f + (blockScale.x / 2f),
                    0,
                    cz * chunkSize * blockScale.z - (gridHeight * blockScale.z) / 2f + (blockScale.z / 2f)
                );

                // Create a container for blocks within this chunk
                GameObject blocksContainer = new GameObject("Blocks_Container");
                blocksContainer.transform.parent = chunk.transform;
                blocksContainer.transform.localPosition = Vector3.zero;
                blocksContainer.layer = LayerMask.NameToLayer(groundLayerName);

                // Instantiate ground blocks within the chunk
                for (int x = 0; x < currentChunkWidth; x++)
                {
                    for (int z = 0; z < currentChunkHeight; z++)
                    {
                        Vector3 position = chunkStartOffset + new Vector3(x * blockScale.x, 0, z * blockScale.z);
                        GameObject block = Object.Instantiate(groundBlockPrefab, position, Quaternion.identity, blocksContainer.transform);
                        block.name = $"Block_{cx * chunkSize + x}_{cz * chunkSize + z}";
                        block.layer = LayerMask.NameToLayer(groundLayerName);
                        block.transform.localScale = blockScale;

                        // Remove individual colliders for optimization
                        Collider[] colliders = block.GetComponents<Collider>();
                        foreach (Collider collider in colliders)
                        {
                            Object.Destroy(collider);
                        }
                    }
                }

                // Combine meshes within this chunk
                CombineChunkMeshes(blocksContainer, chunk);
            }
        }

        Debug.Log($"Successfully created {chunksParent.transform.childCount} chunks of ground blocks.");
    }

    /// <summary>
    /// Combines all ground block meshes within a chunk into a single mesh.
    /// </summary>
    /// <param name="blocksContainer">Container holding all blocks in the chunk.</param>
    /// <param name="chunk">The chunk GameObject.</param>
    private void CombineChunkMeshes(GameObject blocksContainer, GameObject chunk)
    {
        MeshFilter[] meshFilters = blocksContainer.GetComponentsInChildren<MeshFilter>();
        if (meshFilters.Length == 0)
        {
            Debug.LogWarning($"No MeshFilters found in {blocksContainer.name}.");
            return;
        }

        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
        }

        // Create a new GameObject for the combined mesh within the chunk
        GameObject combinedMeshObject = new GameObject("CombinedMesh");
        combinedMeshObject.transform.parent = chunk.transform;
        combinedMeshObject.layer = LayerMask.NameToLayer(groundLayerName);

        MeshFilter mf = combinedMeshObject.AddComponent<MeshFilter>();
        MeshRenderer mr = combinedMeshObject.AddComponent<MeshRenderer>();

        // Combine the meshes
        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combine, true, true);
        mf.mesh = combinedMesh;

        // Assign material
        if (groundMaterial != null)
        {
            mr.material = groundMaterial;
        }
        else
        {
            // If no material is assigned, use the material from the first mesh filter
            MeshRenderer originalRenderer = meshFilters[0].GetComponent<MeshRenderer>();
            if (originalRenderer != null)
            {
                mr.material = originalRenderer.sharedMaterial;
            }
            else
            {
                mr.material = new Material(Shader.Find("Standard"));
                Debug.LogWarning("No Ground Material assigned. Assigned default Standard material.");
            }
        }

        // Optionally, mark as static for optimization
        combinedMeshObject.isStatic = true;

        Debug.Log($"Combined {meshFilters.Length} ground meshes in {chunk.name} into a single mesh.");

        // Optional: Destroy individual blocks to free up memory
        foreach (MeshFilter mfChild in meshFilters)
        {
            Object.Destroy(mfChild.gameObject);
        }
    }

    /// <summary>
    /// Adds a single large BoxCollider to encompass the entire ground grid.
    /// </summary>
    /// <param name="parent">Parent Transform.</param>
    private void AddSingleBoxCollider(Transform parent)
    {
        if (chunksParent == null)
        {
            Debug.LogError("Chunks_Parent not found. Ensure that CreateGroundChunks() is called before AddSingleBoxCollider().");
            return;
        }

        // Calculate the total size based on grid dimensions and block scale
        Vector3 colliderSize = new Vector3(
            gridWidth * blockScale.x,
            blockScale.y,
            gridHeight * blockScale.z
        );

        // Create a new GameObject to hold the collider
        GameObject colliderObject = new GameObject("Ground_CombinedCollider");
        colliderObject.transform.parent = parent;
        colliderObject.layer = LayerMask.NameToLayer(groundLayerName);

        BoxCollider gridCollider = colliderObject.AddComponent<BoxCollider>();
        gridCollider.size = colliderSize;

        // Position the collider at the center of the grid
        gridCollider.center = Vector3.zero;

        // Optionally, mark as static for optimization
        colliderObject.isStatic = true;

        Debug.Log($"Added BoxCollider with size {gridCollider.size} to Ground_CombinedCollider.");
    }
}
