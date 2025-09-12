using UnityEngine;

public class StarInputManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private Camera gameCamera;
    [SerializeField] private StarGraphVisualiser graphVisualiser;
    [SerializeField] private StarGameManager gameManager;
    [SerializeField] private StarAudioManager audioManager;

    private Node<StarData> startNode = null;
    private bool isDragging = false;

    private void Awake()
    {
        if (gameCamera == null) Debug.LogError("Game Camera not assigned to Star Input Manager!");
        if (graphVisualiser == null) Debug.LogError("Graph Visualiser not assigned to Star Input Manager!");
        if (gameManager == null) Debug.LogError("Game Manager not assigned to Star Input Manager!");
    }

    public void HandleInput()
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
            audioManager.PlayNodeClickedSFX();
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
    // prioritises nodes over edges
    private GameObject GetObjectUnderMouse()
    {
        RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll(gameCamera.ScreenPointToRay(Input.mousePosition));

        GameObject nodeHit = null;
        GameObject edgeHit = null;

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider is CircleCollider2D && nodeHit == null)
            {
                nodeHit = hit.collider.gameObject;
            }
            else if (hit.collider is PolygonCollider2D && edgeHit == null)
            {
                edgeHit = hit.collider.gameObject;
            }
        }

        // If player clicked node return it first 
        if (nodeHit != null)
        {
            return nodeHit;
        }
        // Otherwise they must be deleting an edge
        if (edgeHit != null)
        {
 
            return edgeHit;
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
