using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;

public class StarNodeVisual : MonoBehaviour
{
    [SerializeField] private Color normalColor;
    [SerializeField] private Color solutionColor;
    [SerializeField] private Color invalidColor;
    [SerializeField] private Color highlightedColor;

    [Header("Hint Effect")]
    [SerializeField] private GameObject rippleEffectPrefab;
    private ParticleSystem hintParticleSystem;

    [Header("Material Setup")]
    [SerializeField] private Material baseEmissiveMaterial;
    // Controls the intensity of the glow effect
    [SerializeField] private float emissionIntensity = 2.0f;

    private Material starMaterial;
    private bool isActive;
    private bool isSolution;

    public int nodeId { get; private set; }
    public enum NodeState
    {
        None = 0,
        Idle,
        Highlighted,
        ConnectedSolution,
        InvalidConnection,
        Blinking
    }

    public NodeState CurrentState { get; private set; }

    void Awake()
    {
        Renderer renderer = GetComponent<Renderer>();
        starMaterial = new Material(baseEmissiveMaterial);
        renderer.material = starMaterial;
        starMaterial.EnableKeyword("_EMISSION");
    }

    // Set node ID and visual state based on data from the graph class
    public void Initialise(int nodesId, StarData data)
    {
        nodeId = nodesId;
        isSolution = data.IsSolutionNode;
        isActive = true;

        if (starMaterial == null)
        {
            Renderer renderer = GetComponent<Renderer>();
            starMaterial = renderer.material;
        }

        if (rippleEffectPrefab != null && isSolution)
        {
            GameObject rippleInstance = Instantiate(rippleEffectPrefab, transform);
            rippleInstance.transform.localPosition = Vector3.zero;
            hintParticleSystem = rippleInstance.GetComponent<ParticleSystem>();
            hintParticleSystem.Stop();
        }

        SetState(NodeState.Idle);

        if (isSolution)
            name = $"SolutionStar_{nodeId}";
        else
            name = $"Star_{nodeId}";
    }

    public void SetState(NodeState newState)
    {
        if (CurrentState == newState && newState != NodeState.Blinking) return;
        CurrentState = newState;
        UpdateVisuals();
    }

    // Updates node colours based on interaction and game state based on priority
    // highlighted node: turns cyan when player is clicking and dragging a node
    // connected solution node: turns yellow
    // connected non-soltuon node: turns red
    // no connections: default idle colour white
    private void UpdateVisuals()
    {
        if (this == null || !isActive) return;

        // apply all properties for the new state.
        switch (CurrentState)
        {
            case NodeState.Idle:
                SetColor(normalColor);
                break;
            case NodeState.Highlighted:
                SetColor(highlightedColor);
                break;
            case NodeState.ConnectedSolution:
                SetColor(solutionColor);
                break;
            case NodeState.InvalidConnection:
                SetColor(invalidColor);
                break;
            default:
                SetColor(normalColor);
                break;
        }
    }

    public void StartHint()
    {
        if (hintParticleSystem != null)
        {
            hintParticleSystem.Play();
        }
    }

    public void StopHint()
    {
        if (hintParticleSystem != null)
        {
            hintParticleSystem.Stop();
        }
    }

    private void SetColor(Color color)
    {
        if (starMaterial == null)
        {
            Debug.LogError("Star material is null!");
            return;
        }

        //starMaterial.SetColor("_Color",  color);
        starMaterial.SetColor("_EmissionColor", color * emissionIntensity);
    }

    // logic to deactivate to prevent updates on destroyed nodes
    private void OnDestroy()
    {
        isActive = false;
    }
}