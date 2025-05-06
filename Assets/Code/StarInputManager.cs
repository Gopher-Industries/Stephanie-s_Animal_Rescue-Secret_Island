using UnityEngine;

public class StarInputManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private Camera gameCamera;
    [SerializeField] private StarGraphVisualiser graphVisualiser;
    [SerializeField] private StarGameManager gameManager;

    private Node<StarData> startNode = null;
    private bool _isDragging = false;

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

        if (_isDragging && Input.GetMouseButton(0))
        {
            HandleMouseDrag();
        }

        if (Input.GetMouseButtonUp(0) && _isDragging)
        {
            HandleMouseRelease();
        }
    }

    // Start dragging: Find the clicked node and initialize the preview line
    private void HandleMouseDown()
    {
        var hitObject = GetObjectUnderMouse();
        if (hitObject == null) return;

        if (IsEdgeObject(hitObject))
        {
            HandleEdgeClick(hitObject);
            return;
        }

        var clickedNode = gameManager.GetNodeFromGameObject(hitObject);
        if (clickedNode == null) return;

        StartNodeInteraction(clickedNode);
    }

    private void HandleMouseDrag()
    {
        var mouseWorldPos = GetMouseWorldPosition(startNode.position.z);
        graphVisualiser.UpdatePreviewLine(startNode.position, mouseWorldPos);
    }

    // When the mouse is released: Finalize the edge and connect the nodes in the graph
    private void HandleMouseRelease()
    {
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

        ResetInteraction();
    }

    private void StartNodeInteraction(Node<StarData> node)
    {
        startNode = node;
        _isDragging = true;

        if (!node.data.IsSelected)
        {
            gameManager.SetNodeSelected(node.id, true);
            gameManager.UpdateNodeVisualState(node.id);
        }

        graphVisualiser.ClearPreviewLine();
    }

    private void CompleteConnection(Node<StarData> start, Node<StarData> end)
    {
        gameManager.CreateConnection(start.id, end.id);

        gameManager.SetNodeSelected(start.id, true);
        gameManager.SetNodeSelected(end.id, true);

        gameManager.UpdateNodeVisualState(end.id);
        gameManager.UpdateNodeVisualState(start.id);
    }

    private void CleanUpFailedConnection()
    {
        if (startNode == null)
            return;

        if (startNode.neighbours.Count == 0)
        {
            gameManager.SetNodeSelected(startNode.id, false);
        }
        gameManager.UpdateNodeVisualState(startNode.id);
    }

    // dog but works
    private void ResetInteraction()
    {
        graphVisualiser.ClearPreviewLine();
        startNode = null;
        _isDragging = false;
    }

    
    private void HandleEdgeClick(GameObject edgeObject)
    {
        var (nodeA, nodeB) = gameManager.GetNodesFromEdge(edgeObject);
        if (nodeA == null || nodeB == null) 
            return;

        gameManager.RemoveConnection(nodeA.id, nodeB.id);

        //ensure we aren't connected still before updating highlight state
        if (nodeA.neighbours.Count == 0)
            gameManager.SetNodeSelected(nodeA.id, false);
            gameManager.UpdateNodeVisualState(nodeA.id);
        if (nodeB.neighbours.Count == 0)
            gameManager.SetNodeSelected(nodeB.id, false);
            gameManager.UpdateNodeVisualState(nodeB.id);
    }

    #region Utility Methods
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

    private bool IsEdgeObject(GameObject obj)
    {
        return obj.GetComponent<PolygonCollider2D>() != null;
    }
    #endregion
}
