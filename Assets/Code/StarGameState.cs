using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StarGameState
{
    public bool isSolutionValid { get; private set; }
    public int CurrentLevel { get; private set; } = 1;

    public Graph<StarData> currentGraph { get; private set; }
    public List<Node<StarData>> solutionNodes { get; }
    private HashSet<(int, int)> solutionEdges = new HashSet<(int, int)>();

    public void AdvanceLevel()
    {
        CurrentLevel++;
    }

    public StarGameState()
    {
        solutionNodes = new List<Node<StarData>>();
        solutionEdges = new HashSet<(int, int)>();  
    }

    // clear previous graph before initialising
    // cache solution nodes
    public void Initialize(Graph<StarData> graph, List<(int, int)> edges)
    {
        currentGraph = null;
        solutionNodes.Clear();
        solutionEdges.Clear();
        currentGraph = graph;

        foreach (var edge in edges)
        {
            int smallEdge = Mathf.Min(edge.Item1, edge.Item2);
            int largeEdge = Mathf.Max(edge.Item1, edge.Item2);
            solutionEdges.Add((smallEdge, largeEdge));
        }

        foreach (var node in graph.Nodes)
        {
            if (node.data.IsSolutionNode)
            {
                solutionNodes.Add(node);
            }
        }

        isSolutionValid = false;
    }

    public bool TryAddConnection(int nodeAId, int nodeBId)
    {
        var nodeA = FindNode(nodeAId);
        var nodeB = FindNode(nodeBId);

        if (nodeA == null || nodeB == null) return false;

        currentGraph.ConnectNodes(nodeA, nodeB);


        return true;
    }

    public bool TryRemoveConnection(int nodeAId, int nodeBId)
    {
        var nodeA = FindNode(nodeAId);
        var nodeB = FindNode(nodeBId);

        if (nodeA == null || nodeB == null) return false;

        currentGraph.DisconnectNodes(nodeA, nodeB);

        return true;
    }

    // An edge is considered valid if  both nodes are part of the solution edges
    public bool IsEdgeValid(int nodeAId, int nodeBId)
    {
        int min = Mathf.Min(nodeAId, nodeBId);
        int max = Mathf.Max(nodeAId, nodeBId);
        return solutionEdges.Contains((min, max));
    }

    private Node<StarData> FindNode(int id)
    {
        if (currentGraph == null)
        {
            Debug.LogError("Attempted to find node in null graph!");
            return null;
        }

        foreach (var node in currentGraph.Nodes)
        {
            if (node.id == id) return node;
        }

        return null;
    }

    public Node<StarData> GetNodeById(int nodeId)
    {
        if (currentGraph == null) return null;

        foreach (var node in currentGraph.Nodes)
        {
            if (node.id == nodeId) return node;
        }
        return null;
    }

    public void ValidateSolution()
    {
        // Find all connected components first
        var components = FindConnectedComponents(solutionNodes);

        // Then validate each component
        isSolutionValid = components.All(IsClosedShape);
    }

    // Finds all connected components (shapes) within the cached set of solution nodes via DFS
    // Reference: https://www.geeksforgeeks.org/connected-components-in-an-undirected-graph/
    private List<List<Node<StarData>>> FindConnectedComponents(List<Node<StarData>> solutionNodes)
    {
        var visited = new HashSet<Node<StarData>>();
        // list of connected components (shapes)
        var components = new List<List<Node<StarData>>>(); 

        foreach (var node in solutionNodes)
        {
            // if node hasn't been visited its a new component
            if (!visited.Contains(node))
            {
                var component = new List<Node<StarData>>();
                DFS(node, visited, component);
                components.Add(component);
            }
        }

        return components;
    }

    // Validates the current component as a single closed loop of solution nodes
    private bool IsClosedShape(List<Node<StarData>> shapeNodes)
    {
        // Complete shapes require atleast 3 nodes
        if (shapeNodes.Count < 3)
            return false;

        // Each node must have exactly 2 solution node connections to form a loop
        foreach (var node in shapeNodes)
        {
            if (node.neighbours.Count != 2)
                return false;
        }

        // verify that every single connection is a valid "solution edge".
        foreach (var node in shapeNodes)
        {
            foreach (var neighbour in node.neighbours)
            {
                // To avoid checking each edge twice, we only check one direction.
                if (node.id < neighbour.id)
                {
                    if (!IsEdgeValid(node.id, neighbour.id))
                    {
                        // If we find even one connection that isn't part of the solution, the shape is invalid.
                        return false;
                    }
                }
            }
        }

        return true;
    }

    // Ensure all solution nodes are connected as a shape
    // DFS won't visit all nodes if they don't connect the shape
    private void DFS(Node<StarData> currentNode, HashSet<Node<StarData>> visited, List<Node<StarData>> component)
    {
        if (currentNode == null || visited.Contains(currentNode)) return;

        visited.Add(currentNode);
        component.Add(currentNode);

        foreach (var neighbour in currentNode.neighbours)
        {
            if (neighbour.data.IsSolutionNode)
            {
                DFS(neighbour, visited, component);
            }
        }
    }
}