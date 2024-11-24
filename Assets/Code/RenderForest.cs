using UnityEngine;

public class RenderForest : MonoBehaviour
{
    [Header("Tree Prefabs")]
    [Tooltip("Drag your tree prefabs here.")]
    public GameObject[] treePrefabs; // Array to hold your tree prefabs

    [Header("Forest Settings")]
    [Tooltip("Number of trees to generate.")]
    public int numberOfTrees = 100; // Total number of trees to generate

    [Tooltip("Size of the area in which trees will be generated.")]
    public float areaSize = 50f; // The width and length of the area

    [Header("Tree Variation")]
    [Tooltip("Minimum scale multiplier for tree size.")]
    public float minScale = 0.8f;

    [Tooltip("Maximum scale multiplier for tree size.")]
    public float maxScale = 1.2f;

    [Header("Rotation Settings")]
    [Tooltip("Minimum rotation angle around Y-axis.")]
    public float minRotationY = 0f;

    [Tooltip("Maximum rotation angle around Y-axis.")]
    public float maxRotationY = 360f;

    void Start()
    {
        GenerateForest();
    }

    /// <summary>
    /// Generates the forest by instantiating trees at random positions.
    /// </summary>
    void GenerateForest()
    {
        if (treePrefabs == null || treePrefabs.Length == 0)
        {
            Debug.LogError("No tree prefabs assigned in the RenderForest script.");
            return;
        }

        for (int i = 0; i < numberOfTrees; i++)
        {
            // Select a random tree prefab
            GameObject treePrefab = treePrefabs[Random.Range(0, treePrefabs.Length)];

            // Generate a random position within the area
            Vector3 randomOffset = new Vector3(
                Random.Range(-areaSize / 2f, areaSize / 2f),
                0, // We'll adjust Y position later
                Random.Range(-areaSize / 2f, areaSize / 2f)
            );
            Vector3 randomPosition = transform.position + randomOffset;

            // Random rotation around the Y-axis using adjustable range
            float randomYRotation = Random.Range(minRotationY, maxRotationY);
            Quaternion randomRotation = Quaternion.Euler(0, randomYRotation, 0);

            // Random scale variation
            float randomScale = Random.Range(minScale, maxScale);

            // Instantiate the tree
            GameObject treeInstance = Instantiate(treePrefab, randomPosition, randomRotation, transform);

            // Apply scaling
            treeInstance.transform.localScale *= randomScale;

            // Set the tag of the tree to "Tree"
            treeInstance.tag = "Tree";

            // Adjust Y position to keep the base at ground level
            AdjustTreePosition(treeInstance);
        }
    }

    /// <summary>
    /// Adjusts the tree's position so that its base is at ground level after scaling.
    /// </summary>
    /// <param name="tree">The tree GameObject to adjust.</param>
    void AdjustTreePosition(GameObject tree)
    {
        Renderer treeRenderer = tree.GetComponent<Renderer>();
        if (treeRenderer != null)
        {
            Bounds bounds = treeRenderer.bounds;
            // Calculate the offset needed to bring the bottom of the tree to ground level
            float yOffset = bounds.min.y - transform.position.y;
            tree.transform.position -= new Vector3(0, yOffset, 0);
        }
        else
        {
            // If the tree has multiple renderers (e.g., children), get all renderers
            Renderer[] renderers = tree.GetComponentsInChildren<Renderer>();
            if (renderers.Length > 0)
            {
                Bounds combinedBounds = renderers[0].bounds;
                foreach (Renderer renderer in renderers)
                {
                    combinedBounds.Encapsulate(renderer.bounds);
                }
                float yOffset = combinedBounds.min.y - transform.position.y;
                tree.transform.position -= new Vector3(0, yOffset, 0);
            }
            else
            {
                Debug.LogWarning("Tree instance has no Renderer component.");
            }
        }
    }
}
