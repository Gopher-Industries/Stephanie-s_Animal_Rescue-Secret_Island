using System.Collections.Generic;
using UnityEngine;

public class StarGraphVisualiser : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private StarNodeVisual starPrefab;
    [SerializeField] private StarEdgeVisual edgePrefab;
    [SerializeField] private LineRenderer previewLinePrefab;

    private Color lineColor;

    private const float LINE_WIDTH = 0.2f;

    // Maps a nodes Id to their visual representation in the scene
    public Dictionary<int, StarNodeVisual> nodeVisualDict = new Dictionary<int, StarNodeVisual>();

    // Maps node id pairs (undirected edge) to their visual repesented edge in the scene
    public Dictionary<(int, int), StarEdgeVisual> edgeVisualDict = new Dictionary<(int, int), StarEdgeVisual>();


    private LineRenderer previewLine;


    private void Awake()
    {
        // Use ColorUtility to parse your hex color string
        if (!ColorUtility.TryParseHtmlString("#F3F3F3", out lineColor))
        {
            // If the hex code is invalid, it will default to white as a fallback
            lineColor = Color.white;
        }
    }

    // clears then re-renders all node visuals based on the input graph
    public void VisualizeGraph(Graph<StarData> graph)
    {
        ClearVisuals();
        CreateNodeVisuals(graph);
    }

    // Instantiates node GameObjects from graph data and stores them in the dictionary
    // Node IDs increment from 0 to Graph.Count-1
    private void CreateNodeVisuals(Graph<StarData> graph)
    {
        foreach (var node in graph.Nodes)
        {
            StarNodeVisual starVisual = Instantiate(starPrefab, node.position, Quaternion.identity, transform);
            starVisual.Initialise(node.id, node.data);
            nodeVisualDict[node.id] = starVisual;
        }
    }

    // Instantiates and renders an edge between two nodes
    public StarEdgeVisual DrawEdge(Node<StarData> a, Node<StarData> b)
    {
        if (EdgeExists(a.id, b.id)) return null;

        var edge = Instantiate(edgePrefab, transform);
        edge.Initialize(a.position, b.position, LINE_WIDTH);
        AddEdgeToDict(a.id, b.id, edge);

        return edge;
    }

    // Removes the visual connection between two nodes
    public void RemoveEdge(Node<StarData> a, Node<StarData> b)
    {
        if (!TryGetEdge(a.id, b.id, out var edge)) 
            return;

        Destroy(edge.gameObject);
        RemoveEdgeFromDict(a.id, b.id);
        
    }

    // Displays a temporary line between the start node and mouse position when player is dragging mouse
    public void UpdatePreviewLine(Vector3 startPos, Vector3 endPos)
    {
        if (previewLine == null)
        {
            previewLine = Instantiate(previewLinePrefab, transform);
            previewLine.startColor = lineColor;
            previewLine.endColor = lineColor;
            previewLine.startWidth = LINE_WIDTH;
            previewLine.endWidth = LINE_WIDTH;
            previewLine.widthMultiplier = LINE_WIDTH;
            previewLine.alignment = LineAlignment.TransformZ;
        }

        previewLine.SetPositions(new Vector3[] { startPos, endPos });
    }

    // Clears the existing preview line used during drag interaction
    public void ClearPreviewLine()
    {
        if (previewLine != null)
        {
            Destroy(previewLine.gameObject);
            previewLine = null;
        }
    }

    // Removes all node and edge visuals from the scene and resets tracking dictionaries
    public void ClearVisuals()
    {
        foreach (var visual in nodeVisualDict.Values) Destroy(visual.gameObject);
        foreach (var edge in edgeVisualDict.Values) Destroy(edge.gameObject);

        nodeVisualDict.Clear();
        edgeVisualDict.Clear();
        ClearPreviewLine();
    }

    // Returns true if the given GameObject matches the visual of the specified node ID
    public bool DoesVisualRepresentNode(GameObject gameObject, int nodeId)
    {
        return nodeVisualDict.TryGetValue(nodeId, out var nodeVisual) &&
               nodeVisual.gameObject == gameObject;
    }

    #region Helper Methods

    // Checks if an undirected edge already exists between two node IDs
    private bool EdgeExists(int idA, int idB)
    {
        if (edgeVisualDict.ContainsKey((idA, idB)))
            return true;

        if (edgeVisualDict.ContainsKey((idB, idA)))
            return true;

        return false;
    }

    // Attempts to retrieve the visual edge between two nodes, regardless of direction
    private bool TryGetEdge(int idA, int idB, out StarEdgeVisual edge)
    {
        if (edgeVisualDict.TryGetValue((idA, idB), out edge)) 
            return true;

        if (edgeVisualDict.TryGetValue((idB, idA), out edge)) 
            return true;

        return false;
    }

    // Adds the edge to the dictionary in both directions for bidirectional lookup
    private void AddEdgeToDict(int idA, int idB, StarEdgeVisual edge)
    {
        edgeVisualDict[(idA, idB)] = edge;
        edgeVisualDict[(idB, idA)] = edge; 
    }

    // Removes both directional keys for an edge from the dictionary
    private void RemoveEdgeFromDict(int idA, int idB)
    {
        edgeVisualDict.Remove((idA, idB));
        edgeVisualDict.Remove((idB, idA));
    }
    #endregion
}
