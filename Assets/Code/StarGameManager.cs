using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StarGameManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private StarLevelGenerator levelGenerator;
    [SerializeField] private StarGraphVisualiser visualiser;
    [SerializeField] private StarInputManager inputManager;
    [SerializeField] private StarUIManager uiManager;
    [SerializeField] private StarAudioManager audioManager;

    private StarGameState gameState;
    private GameState currentState;

    private const int MAX_LEVELS = 8;
    private const float IDLE_THRESHOLD = 5f;
    private float lastInteractionTime;
    private List<StarNodeVisual> activeHintNodes = new List<StarNodeVisual>();


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
            CheckForIdlePlayer();
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
        ResetIdleTimer();

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
        audioManager.PlayLevelCompleteSFX();

        ClearAllHints();
        ResetIdleTimer();
    }

    private void GameComplete()
    {
        uiManager.ShowGameCompleteScreen();
        uiManager.SetPreviewButtonVisibility(false);
        audioManager.PlayLevelCompleteSFX();
    }

    // after a level is complete clears the screen
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
            // Any correct action resets the hint timer.
            bool isCorrectConnection = gameState.IsEdgeValid(nodeAId, nodeBId);
            if (isCorrectConnection)
            {
                ResetIdleTimer();

                // When Player connects a hinted node correctly.
                // Check if either node was being hinted at and remove the hint.
                RemoveHintFromNode(nodeAId);
                RemoveHintFromNode(nodeBId);
            }
            StarEdgeVisual edge = visualiser.DrawEdge(nodeA, nodeB);
            if (edge == null) return;
            UpdateEdgeVisualState(edge, nodeAId, nodeBId);
            audioManager.PlayConnectionSFX();
            UpdateNodeVisualState(nodeAId);
            UpdateNodeVisualState(nodeBId);
            gameState.ValidateSolution();
        }
    }

    public void RemoveConnection(int nodeAId, int nodeBId)
    {
        bool wasSolutionEdge = gameState.IsEdgeValid(nodeAId, nodeBId);

        Node<StarData> nodeA = gameState.GetNodeById(nodeAId);
        Node<StarData> nodeB = gameState.GetNodeById(nodeBId);

        if (gameState.TryRemoveConnection(nodeAId, nodeBId))
        {
            lastInteractionTime = Time.time;
            visualiser.RemoveEdge(nodeA, nodeB);
            audioManager.PlayDisconnectionSFX();
            UpdateNodeVisualState(nodeAId);
            UpdateNodeVisualState(nodeBId);
            gameState.ValidateSolution();
        }
    }

    // Check for stuck player, if they haven't made a correcet move in x seconds
    // trigger the hint
    private void CheckForIdlePlayer()
    {
        if (Time.time - lastInteractionTime > IDLE_THRESHOLD)
        {
            TriggerNextHint();
            lastInteractionTime = Time.time; 
        }
    }

    private void TriggerNextHint()
    {
        // Find all valid hint candidates (unsolved and not already hinted)
        var candidates = gameState.solutionNodes.Where(node =>
            node.data.IsSolutionNode &&
            node.neighbours.Count(neighbour => neighbour.data.IsSolutionNode) < 2 &&
            !activeHintNodes.Any(hintedNode => hintedNode.nodeId == node.id));

        // Sort the candidates to prioritise the node with the fewest connections
        var hintNode = candidates
            .OrderBy(node => node.neighbours.Count(n => n.data.IsSolutionNode))
            .FirstOrDefault();

        if (hintNode != null && visualiser.nodeVisualDict.TryGetValue(hintNode.id, out var nodeVisual))
        {
            // We now call the dedicated hint method.
            nodeVisual.StartHint();
            activeHintNodes.Add(nodeVisual);
        }
    }

    // Remove a hint from a specific node if it's active
    public void RemoveHintFromNode(int nodeId)
    {
        StarNodeVisual hintToRemove = activeHintNodes.FirstOrDefault(n => n.nodeId == nodeId);
        if (hintToRemove != null)
        {
            hintToRemove.StopHint();
            // reset its colour state
            hintToRemove.SetState(StarNodeVisual.NodeState.Idle);
            activeHintNodes.Remove(hintToRemove);
        }
    }

    private void ResetIdleTimer()
    {
        lastInteractionTime = Time.time;
    }

    // clear all hints between levels
    private void ClearAllHints()
    {
        foreach (var nodeVisual in activeHintNodes)
        {
            if (nodeVisual != null)
            {
                nodeVisual.StopHint();
                nodeVisual.SetState(StarNodeVisual.NodeState.Idle);
            }
        }
        activeHintNodes.Clear();
    }

    // Updates the nodes state based on what connection was found
    public void UpdateNodeVisualState(int nodeId)
    {
        Node<StarData> node = gameState.GetNodeById(nodeId);
        if (node == null || !visualiser.nodeVisualDict.TryGetValue(nodeId, out var nodeVisual))
            return;

        // no connections defaults to idle
        if (node.neighbours.Count == 0)
        {
            nodeVisual.SetState(StarNodeVisual.NodeState.Idle);
            return;
        }

        // check if this node is part of solution node
        if (node.data.IsSolutionNode)
        {
            // A solution node connected to ANY non solution node is an invalid connection
            if (node.neighbours.Any(neighbour => !neighbour.data.IsSolutionNode))
            {
                nodeVisual.SetState(StarNodeVisual.NodeState.InvalidConnection);
                return;
            }

            bool allEdgesAreValid = node.neighbours.All(neighbour => gameState.IsEdgeValid(nodeId, neighbour.id));
            if (allEdgesAreValid)
            {
                // solution node with only solution connections
                nodeVisual.SetState(StarNodeVisual.NodeState.ConnectedSolution);
            }
            else
            {
                // if one connection is a non solution star it's an invalid connection
                nodeVisual.SetState(StarNodeVisual.NodeState.InvalidConnection);
            }
        }
        else
        {
            // node wasn't a solution node
            nodeVisual.SetState(StarNodeVisual.NodeState.InvalidConnection);
        }
    }

    public void UpdateEdgeVisualState(StarEdgeVisual edge, int nodeAId, int nodeBId)
    {
        if (edge == null) return;

        Color solutionEdgeColour = Color.yellow;
        Color incorrectSolutionColour = Color.red;

        bool isValidEdge = gameState.IsEdgeValid(nodeAId, nodeBId);

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