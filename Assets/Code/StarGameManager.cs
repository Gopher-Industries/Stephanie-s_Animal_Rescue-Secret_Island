using UnityEngine;

public class StarGameManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private StarLevelGenerator levelGenerator;
    [SerializeField] private StarGraphVisualiser visualiser;
    [SerializeField] private StarInputManager inputManager;
    [SerializeField] private StarUIManager uiManager;

    private StarGameState gameState;

    private void Awake()
    {
        if (levelGenerator == null) Debug.LogError("Level Generator not assigned to Star Game Manager!");
        if (visualiser == null) Debug.LogError("Visualiser not assigned to Star Game Manager!");
        if (inputManager == null) Debug.LogError("Input Manager not assigned to Star Game Manager!");
        if (uiManager == null) Debug.LogError("UI Manager not assigned to Star Game Manager!");

        gameState = new StarGameState();
        Graph<StarData> starGraph = levelGenerator.GenerateLevel();
        gameState.Initialize(starGraph);
    }

    private void Start()
    {
        StartNewLevel();
    }

    public void StartNewLevel()
    {
        var newGraph = levelGenerator.GenerateLevel();
        gameState.Initialize(newGraph);
        visualiser.VisualizeGraph(newGraph);
        uiManager.ShowPreview(ShapeType.Square);
    }

    private void ValidateCurrentLevel()
    {
        if (gameState.isSolutionValid)
        {
            Debug.Log("Level Complete!");
            // TODO: Create level 2
        }
    }

    public void CreateConnection(int nodeAId, int nodeBId)
    {
        Node<StarData> nodeA = gameState.GetNodeById(nodeAId);
        Node<StarData> nodeB = gameState.GetNodeById(nodeBId);

        if (gameState.TryAddConnection(nodeAId, nodeBId))
        {
            visualiser.DrawEdge(nodeA, nodeB);
            UpdateNodeVisualState(nodeAId);
            UpdateNodeVisualState(nodeBId);
        }
    }

    public void RemoveConnection(int nodeAId, int nodeBId)
    {
        Node<StarData> nodeA = gameState.GetNodeById(nodeAId);
        Node<StarData> nodeB = gameState.GetNodeById(nodeBId);

        if (gameState.TryRemoveConnection(nodeAId, nodeBId))
        {
            visualiser.RemoveEdge(nodeA, nodeB);
            UpdateNodeVisualState(nodeAId);
            UpdateNodeVisualState(nodeBId);
        }

    }

    public void SetNodeSelected(int nodeId, bool selected)
    {
        gameState.ToggleNodeSelection(nodeId, selected);
    }

    public void UpdateNodeVisualState(int nodeId)
    {
        var node = gameState.GetNodeById(nodeId);
        if (node == null) 
            return;

        bool hasConnections = node.neighbours.Count > 0;
        bool isHighlighted = node.data.IsSelected;

        visualiser.UpdateNodeHighlight(nodeId, isHighlighted, hasConnections);
    }

    public (Node<StarData>, Node<StarData>) GetNodesFromEdge(GameObject edgeObject)
    {
        foreach (var visualPair in visualiser.edgeVisuals)
        {
            if (visualPair.Value.gameObject == edgeObject)
            {
                return (gameState.GetNodeById(visualPair.Key.Item1),
                       gameState.GetNodeById(visualPair.Key.Item2));
            }
        }
        return (null, null);
    }

    public Node<StarData> GetNodeFromGameObject(GameObject gameObject)
    {
        foreach (var node in gameState.currentGraph.Nodes)
        {
            if (visualiser.DoesVisualRepresentNode(gameObject, node.id))
                return node;
        }
        return null;
    }
}