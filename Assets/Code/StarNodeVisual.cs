using UnityEngine;

public class StarNodeVisual : MonoBehaviour
{
    [SerializeField] private Color normalColor = Color.yellow;
    [SerializeField] private Color solutionColor = Color.green;
    [SerializeField] private Color invalidColor = Color.red;


    private Material starMaterial;
    private bool isSolution;
    private bool hasConnections;

    private const float BRIGHTNESS = 2f;

    public int nodeId { get; private set; }

    public void Initialize(int nodesId, StarData data)
    {
        nodeId = nodesId;
        isSolution = data.IsSolutionNode;

        starMaterial = GetComponent<Renderer>().material;
        starMaterial.EnableKeyword("_EMISSION");
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
        starMaterial.color = color;
        starMaterial.SetColor("_EmissionColor", color * BRIGHTNESS);
    }
}

