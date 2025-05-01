using UnityEngine;

public class StarGameManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private StarLevelGenerator levelGenerator;
    [SerializeField] private StarGraphVisualizer visualizer;
    [SerializeField] private StarInputManager inputManager;

    private StarGameState gameState;

    private void Awake()
    {
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
        visualizer.VisualizeGraph(newGraph);
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
            visualizer.DrawEdge(nodeA, nodeB);
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
            visualizer.RemoveEdge(nodeA, nodeB);
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

        visualizer.UpdateNodeHighlight(nodeId, isHighlighted, hasConnections);
    }

    public (Node<StarData>, Node<StarData>) GetNodesFromEdge(GameObject edgeObject)
    {
        foreach (var visualPair in visualizer.edgeVisuals)
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
            if (visualizer.DoesVisualRepresentNode(gameObject, node.id))
                return node;
        }
        return null;
    }
}