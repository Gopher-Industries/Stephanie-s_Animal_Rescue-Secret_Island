using UnityEngine;

public class StarInputManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private Camera gameCamera;
    [SerializeField] private StarGraphVisualiser graphVisualiser;
    [SerializeField] private StarGameManager gameManager;

    private Node<StarData> startNode = null;
    private bool isDragging = false;

    private void Awake()
    {
        if (gameCamera == null) Debug.LogError("Game Camera not assigned to Star Input Manager!");
        if (graphVisualiser == null) Debug.LogError("Graph Visualiser not assigned to Star Input Manager!");
        if (gameManager == null) Debug.LogError("Game Manager not assigned to Star Input Manager!");
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseDown();
        }

        if (isDragging && Input.GetMouseButton(0))
        {
            HandleMouseDrag();
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            HandleMouseRelease();
        }
    }

    // Find the clicked node or edge via RayCast
    // Clicking a node highlights the node & initialises isDragging flag
    // Clicking on an edge deletes the edge and it's connection
    private void HandleMouseDown()
    {
        var hitObject = GetObjectUnderMouse();
        if (hitObject == null) return;

        if (HasPolygonCollider2D(hitObject))
        {
            HandleEdgeClick(hitObject);
            return;
        }

        var clickedNode = gameManager.GetNodeFromGameObject(hitObject);
        if (clickedNode == null) return;

        StartNodeInteraction(clickedNode);
    }

    // While holding left mouse button down update the preview line positon to track mouse positon
    private void HandleMouseDrag()
    {
        var mouseWorldPos = GetMouseWorldPosition(startNode.position.z);
        graphVisualiser.UpdatePreviewLine(startNode.position, mouseWorldPos);
    }

    // When the mouse is released: Finalize the edge and connect the nodes in the graph
    // Won't connect an edge if they release on the same node or released over empty spac
    private void HandleMouseRelease()
    {
        if (startNode == null)
        {
            CleanUpFailedConnection();
            return;
        }

        var hitObject = GetObjectUnderMouse();

        var releasedNode = hitObject != null ? gameManager.GetNodeFromGameObject(hitObject) : null;

        if (releasedNode != null && releasedNode != startNode)
        {
            CompleteConnection(startNode, releasedNode);
        }
        else
        {
            CleanUpFailedConnection();
        }

        ClearSelection();
    }

    private void StartNodeInteraction(Node<StarData> node)
    {
        startNode = node;
        isDragging = true;

        if (graphVisualiser.nodeVisualDict.TryGetValue(node.id, out var visual))
        {
            visual.SetHighlightedState(true);
        }

        graphVisualiser.ClearPreviewLine();
    }

    private void CompleteConnection(Node<StarData> start, Node<StarData> end)
    {
        gameManager.CreateConnection(start.id, end.id);

        gameManager.UpdateNodeVisualState(end.id);
        gameManager.UpdateNodeVisualState(start.id);
    }

    private void CleanUpFailedConnection()
    {
        if (startNode == null)
            return;

        if (graphVisualiser.nodeVisualDict.TryGetValue(startNode.id, out var nodeVisual))
        {
            nodeVisual.SetHighlightedState(false);
            nodeVisual.UpdateColourState(startNode.neighbours.Count > 0);
        }

        graphVisualiser.ClearPreviewLine();

    }

    // dog but works
    // resets the isDragging state and visual highlight
    public void ClearSelection()
    {
        if (startNode != null && graphVisualiser.nodeVisualDict.TryGetValue(startNode.id, out var visual))
        {
            visual.SetHighlightedState(false);
            visual.UpdateColourState(startNode.neighbours.Count > 0);
        }

        graphVisualiser.ClearPreviewLine();
        startNode = null;
        isDragging = false;
    }

    // Handles removal of existing edge connection when hide is clicked
    // Only update node visual state if node has no remaining neighbors
    private void HandleEdgeClick(GameObject edgeObject)
    {
        var (nodeA, nodeB) = gameManager.GetNodesFromEdge(edgeObject);
        if (nodeA == null || nodeB == null) 
            return;

        gameManager.RemoveConnection(nodeA.id, nodeB.id);

        //ensure we aren't connected still before updating highlight state
        if (nodeA.neighbours.Count == 0)
        {
            gameManager.UpdateNodeVisualState(nodeA.id);
        }
        if (nodeB.neighbours.Count == 0)
        {
            gameManager.UpdateNodeVisualState(nodeB.id);
        }
    }


    #region Utility Methods

    // Raycast for object under mouse
    // supports both 3D and 2D colliders
    // prioritises 3D as star node is 3D
    // might break if we change to sprites
    private GameObject GetObjectUnderMouse()
    {
        RaycastHit hit3D;
        if (Physics.Raycast(gameCamera.ScreenPointToRay(Input.mousePosition), out hit3D))
        {
            return hit3D.collider.gameObject;
        }

        RaycastHit2D hit2D = Physics2D.GetRayIntersection(gameCamera.ScreenPointToRay(Input.mousePosition));
        if (hit2D.collider != null)
        {
            return hit2D.collider.gameObject;
        }

        return null;
    }

    private Vector3 GetMouseWorldPosition(float zDepth)
    {
        return gameCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, zDepth));
    }

    // Edges only use the PolygonCollider2D 
    // Nodes use the Box Collider
    private bool HasPolygonCollider2D(GameObject obj)
    {
        return obj.GetComponent<PolygonCollider2D>() != null;
    }
    #endregion
}
