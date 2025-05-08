using UnityEngine;
using UnityEngine.Events;
using System.Collections;

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

        InitializeGame();
    }

    private void InitializeGame()
    {
        gameState = new StarGameState();
        StartNewLevel();
    }

    public void StartNewLevel()
    {
        visualiser.ClearVisuals();
        inputManager.ClearSelection();

        var newGraph = levelGenerator.GenerateLevel(gameState.CurrentLevel);
        gameState.Initialize(newGraph);
        visualiser.VisualizeGraph(newGraph);
        uiManager.ShowPreview(GetShapeForLevel(gameState.CurrentLevel));

        Debug.Log($"Started Level {gameState.CurrentLevel}");
    }

    private ShapeType GetShapeForLevel(int level)
    {
        switch(level)
        {
            case 1:
                return ShapeType.Triangle;
            case 2:
                return ShapeType.Triangle;
            case 3:
                return ShapeType.Square;
            case 4:
                return ShapeType.Square;
            default:
                return ShapeType.Triangle;
        }
    }

    // delay level transition 1 frame to work around bug that causes last connected nodes to highlight after level transition
    private void ValidateCurrentLevel()
    {
        if (gameState.isSolutionValid)
        {
            Debug.Log($"Level {gameState.CurrentLevel} Complete!");

            foreach (var node in gameState.solutionNodes)
            {
                UpdateNodeVisualState(node.id);
            }


            StartCoroutine(TransitionLevel());
        }
    }

    private IEnumerator TransitionLevel()
    {
        yield return null; 

        inputManager.ClearSelection();
        visualiser.ClearVisuals();
        uiManager.HidePreview();

        gameState.AdvanceLevel();
        StartNewLevel();
    }

    public void CreateConnection(int nodeAId, int nodeBId)
    {
        Node<StarData> nodeA = gameState.GetNodeById(nodeAId);
        Node<StarData> nodeB = gameState.GetNodeById(nodeBId);

        if (gameState.TryAddConnection(nodeAId, nodeBId))
        {
            StarEdgeVisual edge = visualiser.DrawEdge(nodeA, nodeB);
            if (edge == null) return;
            UpdateEdgeVisualState(edge, nodeAId, nodeBId);

            UpdateNodeVisualState(nodeAId);
            UpdateNodeVisualState(nodeBId);
            gameState.ValidateSolution();
            ValidateCurrentLevel();
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
            gameState.ValidateSolution();
            ValidateCurrentLevel();
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

    public void UpdateEdgeVisualState(StarEdgeVisual edge, int nodeAId, int nodeBId)
    {
        if (edge == null) return;

        Color solutionEdgeColour = Color.green;
        Color incorrectSolutionColour = Color.red;

        bool isValidEdge = gameState.IsEdgeValid(nodeAId, nodeBId);
        Debug.Log($"Edge is: {isValidEdge}");

        if (isValidEdge)
        {
            edge.SetColour(solutionEdgeColour);
        }    
        else
        {
            edge.SetColour(incorrectSolutionColour);
        }
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