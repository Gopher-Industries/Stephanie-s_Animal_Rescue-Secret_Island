using UnityEngine;

public class StarNodeVisual : MonoBehaviour
{
    [SerializeField] private Color normalColor = Color.yellow;
    [SerializeField] private Color solutionColor = Color.green;
    [SerializeField] private Color invalidColor = Color.red;

    private Material starMaterial;
    private bool isSolution;
    private bool hasConnections;

    public int nodeId { get; private set; }

    void Awake()
    {
        Renderer renderer = GetComponent<Renderer>();
        starMaterial = new Material(Shader.Find("Unlit/Color"));
        renderer.material = starMaterial;
    }

    public void Initialize(int nodesId, StarData data)
    {
        nodeId = nodesId;
        isSolution = data.IsSolutionNode;

        if (starMaterial == null)
        {
            Renderer renderer = GetComponent<Renderer>();
            starMaterial = renderer.material;
        }

        UpdateHighlightState();

        if (isSolution)
            name = $"SolutionStar_{nodeId}";
        else
            name = $"Star_{nodeId}";
    }

    public void UpdateHighlightState(bool isHighlighted = false, bool hasConnections = false)
    {
        this.hasConnections = hasConnections;

        if (isSolution && hasConnections)
        {
            SetColor(solutionColor);
        }
        else if (isHighlighted)
        {
            SetColor(invalidColor);
        }
        else
        {
            SetColor(normalColor);
        }
    }

    private void SetColor(Color color)
    {
        if (starMaterial == null)
        {
            Debug.LogError("Star material is null!");
            return;
        }

        starMaterial.color = color;
    }
}