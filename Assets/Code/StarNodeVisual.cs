using UnityEngine;

public class StarNodeVisual : MonoBehaviour
{
    [SerializeField] private Color normalColor = Color.yellow;
    [SerializeField] private Color solutionColor = Color.green;
    [SerializeField] private Color invalidColor = Color.red;
    [SerializeField] private Color highlightedColor = Color.cyan;

    private Material starMaterial;
    private bool isActive;
    private bool isSolution;
    private bool hasConnections;
    private bool isHighlighted;

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
        isActive = true;
        isHighlighted = false;

        if (starMaterial == null)
        {
            Renderer renderer = GetComponent<Renderer>();
            starMaterial = renderer.material;
        }

        UpdateColourState();

        if (isSolution)
            name = $"SolutionStar_{nodeId}";
        else
            name = $"Star_{nodeId}";
    }

    public void SetHighlightedState(bool highlighted)
    {
        isHighlighted = highlighted;
        UpdateColourState();
    }

    public void UpdateColourState(bool hasConnections = false)
    {
        if (this == null || !isActive)
            return;

        this.hasConnections = hasConnections;

        if (isHighlighted)
        {
            SetColor(highlightedColor);
        }
        else if (isSolution && hasConnections)
        {
            SetColor(solutionColor);
        }
        else if (hasConnections)
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

    private void OnDestroy()
    {
        isActive = false;
    }
}