using UnityEngine;

public class StarGameManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private StarLevelGenerator levelGenerator;
    [SerializeField] private StarGraphVisualiser visualiser;
    [SerializeField] private StarInputManager inputManager;
    [SerializeField] private StarUIManager uiManager;

    private StarGameState gameState;
    private GameState currentState;

    private const int MAX_LEVELS = 8;

    private enum GameState
    {
        None = 0,
        GameInitialise,
        Playing,
        ShowPreview,
        LevelComplete,
        GameComplete
    }

#if UNITY_EDITOR
    private void Awake()
    {
        if (levelGenerator == null) Debug.LogError("Level Generator not assigned to Star Game Manager!");
        if (visualiser == null) Debug.LogError("Visualiser not assigned to Star Game Manager!");
        if (inputManager == null) Debug.LogError("Input Manager not assigned to Star Game Manager!");
        if (uiManager == null) Debug.LogError("UI Manager not assigned to Star Game Manager!");
    }
#endif

    private void Start()
    {
        gameState = new StarGameState();
        StateSet(GameState.GameInitialise);
    }

    private void Update()
    {
        if (currentState == GameState.Playing)
        {
            GameRun();
        }
    }

    // This runs the initialisation logic for each state
    private void StateSet(GameState newState)
    {
        if (currentState == newState) return;

        currentState = newState;

        switch (currentState)
        {
            case GameState.GameInitialise:
                GameLevelInitialise();
                break;
            case GameState.Playing:
                // No setup needed handled in update
                break;
            case GameState.LevelComplete:
                GameLevelComplete(); 
                break;
            case GameState.GameComplete:
                GameComplete();
                break;
        }
    }


    private void GameLevelInitialise()
    {
        visualiser.ClearVisuals();
        inputManager.ClearSelection();
        var (newGraph, solutionEdges, shapes) = levelGenerator.GenerateLevel(gameState.CurrentLevel);
        gameState.Initialize(newGraph, solutionEdges);
        visualiser.VisualizeGraph(newGraph);
        uiManager.DisplayShapePreviews(shapes);
        uiManager.SetPreviewButtonVisibility(true);

        Debug.Log($"Started Level {gameState.CurrentLevel}");
        StateSet(GameState.Playing);

    }

    // This runs every frame
    private void GameRun()
    {
        inputManager.HandleInput();

        if (gameState.isSolutionValid)
        {
            if (gameState.CurrentLevel == MAX_LEVELS)
            {
                StateSet(GameState.GameComplete);
            }
            else
            {
                StateSet(GameState.LevelComplete);
            }
        }
    }

    public void TogglePreviewState()
    {
        if (currentState == GameState.Playing)
        {
            StateSet(GameState.ShowPreview);
            uiManager.ToggleShapePreview();
        }
        else if (currentState == GameState.ShowPreview)
        {
            StateSet(GameState.Playing);
            uiManager.ToggleShapePreview();
        }
    }

    private void GameLevelComplete()
    {
        uiManager.LevelCompleteShow(gameState.CurrentLevel);
        uiManager.SetPreviewButtonVisibility(false);
    }

    private void GameComplete()
    {
        uiManager.ShowGameCompleteScreen();
        uiManager.SetPreviewButtonVisibility(false);
    }

    public void TransitionLevel()
    {
        inputManager.ClearSelection();
        visualiser.ClearVisuals();
        uiManager.LevelCompleteHide();
        uiManager.SetPreviewButtonVisibility(false);

        gameState.AdvanceLevel();
        StateSet(GameState.GameInitialise);
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
        }

    }

    public void UpdateNodeVisualState(int nodeId)
    {

        var node = gameState.GetNodeById(nodeId);
        if (node == null) 
            return;

        bool hasConnections = node.neighbours.Count > 0;

        visualiser.UpdateNodeColour(nodeId, hasConnections);
    }

    public void UpdateEdgeVisualState(StarEdgeVisual edge, int nodeAId, int nodeBId)
    {
        if (edge == null) return;

        Color solutionEdgeColour = Color.green;
        Color incorrectSolutionColour = Color.red;

        bool isValidEdge = gameState.IsEdgeValid(nodeAId, nodeBId);
        //Debug.Log($"Edge is: {isValidEdge}");

        if (isValidEdge)
        {
            edge.SetColour(solutionEdgeColour);
        }    
        else
        {
            edge.SetColour(incorrectSolutionColour);
        }
    }

    // looks up node pair associated with the given edge gameobject
    public (Node<StarData>, Node<StarData>) GetNodesFromEdge(GameObject edgeObject)
    {
        foreach (var visualPair in visualiser.edgeVisualDict)
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