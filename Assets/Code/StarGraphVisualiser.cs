using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class StarGraphVisualiser : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private StarNodeVisual starPrefab;
    [SerializeField] private StarEdgeVisual edgePrefab;
    [SerializeField] private LineRenderer previewLinePrefab;

    private Color lineColor = Color.yellow;

    private const float LINE_WIDTH = 0.25f;

    // Maps Node<StarData> to its corresponding GameObject
    public Dictionary<int, StarNodeVisual> nodeVisualDict = new Dictionary<int, StarNodeVisual>();

    // Tracks if we have drawn an edge and stores it's GameObject, the key is the a and b node.
    public Dictionary<(int, int), StarEdgeVisual> edgeVisualDict = new Dictionary<(int, int), StarEdgeVisual>();


    private LineRenderer previewLine;

    public void VisualizeGraph(Graph<StarData> graph)
    {
        ClearVisuals();
        CreateNodeVisuals(graph);
    }

    private void CreateNodeVisuals(Graph<StarData> graph)
    {
        int nodeId = 1;
        foreach (var node in graph.Nodes)
        {
            var starVisual = Instantiate(starPrefab, node.position, Quaternion.identity, transform);
            starVisual.Initialize(nodeId, node.data);
            nodeVisualDict[node.id] = starVisual;
            nodeId++;
        }
    }

    public StarEdgeVisual DrawEdge(Node<StarData> a, Node<StarData> b)
    {
        if (EdgeExists(a.id, b.id)) return null;

        var edge = Instantiate(edgePrefab, transform);
        edge.Initialize(a.position, b.position, LINE_WIDTH);
        StoreEdge(a.id, b.id, edge);

        return edge;
    }

    public void RemoveEdge(Node<StarData> a, Node<StarData> b)
    {
        if (!TryGetEdge(a.id, b.id, out var edge)) 
            return;

        Destroy(edge.gameObject);
        RemoveEdgeFromDict(a.id, b.id);
        
    }

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

    // Clear any existing preview line
    public void ClearPreviewLine()
    {
        if (previewLine != null)
        {
            Destroy(previewLine.gameObject);
            previewLine = null;
        }
    }

    public void UpdateNodeColour(int nodeId, bool hasConnections)
    {
        if (nodeVisualDict == null || nodeVisualDict.Count == 0)
            return;

        if (nodeVisualDict.TryGetValue(nodeId, out var nodeVisual))
        {
            nodeVisual.UpdateColourState(hasConnections);
        }
    }

    public void ClearVisuals()
    {
        foreach (var visual in nodeVisualDict.Values) Destroy(visual.gameObject);
        foreach (var edge in edgeVisualDict.Values) Destroy(edge.gameObject);

        nodeVisualDict.Clear();
        edgeVisualDict.Clear();
        ClearPreviewLine();
    }

    public bool DoesVisualRepresentNode(GameObject gameObject, int nodeId)
    {
        return nodeVisualDict.TryGetValue(nodeId, out var nodeVisual) &&
               nodeVisual.gameObject == gameObject;
    }

    #region Helper Methods
    private bool EdgeExists(int idA, int idB)
    {
        if (edgeVisualDict.ContainsKey((idA, idB)))
            return true;

        if (edgeVisualDict.ContainsKey((idB, idA)))
            return true;

        return false;
    }

    private bool TryGetEdge(int idA, int idB, out StarEdgeVisual edge)
    {
        if (edgeVisualDict.TryGetValue((idA, idB), out edge)) 
            return true;

        if (edgeVisualDict.TryGetValue((idB, idA), out edge)) 
            return true;

        return false;
    }

    private void StoreEdge(int idA, int idB, StarEdgeVisual edge)
    {
        edgeVisualDict[(idA, idB)] = edge;
        edgeVisualDict[(idB, idA)] = edge; 
    }

    private void RemoveEdgeFromDict(int idA, int idB)
    {
        edgeVisualDict.Remove((idA, idB));
        edgeVisualDict.Remove((idB, idA));
    }
    #endregion
}
