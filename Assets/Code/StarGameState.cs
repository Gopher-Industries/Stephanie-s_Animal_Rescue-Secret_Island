using System.Collections.Generic;
using UnityEngine;

public class StarGameState
{
    public Graph<StarData> currentGraph { get; private set; }
    public List<Node<StarData>> solutionNodes { get; }
    public bool isSolutionValid { get; private set; }

    public int CurrentLevel { get; private set; } = 1;

    public void AdvanceLevel()
    {
        CurrentLevel++;
    }

    public StarGameState()
    {
        solutionNodes = new List<Node<StarData>>();
    }

    public void Initialize(Graph<StarData> graph)
    {
        currentGraph = null;
        solutionNodes.Clear();
        currentGraph = graph;

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

    public bool ToggleNodeSelection(int nodeId, bool selected)
    {
        Node<StarData> node = FindNode(nodeId);

        if (node == null)
        {
            return false;
        }

        node.data.IsSelected = selected;
        return true;
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
        isSolutionValid = CheckSolution(solutionNodes);
    }

    private bool CheckSolution(List<Node<StarData>> solutionNodes)
    {
        if (solutionNodes.Count < 3)
            return false;

        HashSet<Node<StarData>> visited = new HashSet<Node<StarData>>();
        DFS(solutionNodes[0], visited);


        if (visited.Count != solutionNodes.Count)
            return false;


        foreach (var node in solutionNodes)
        {
            foreach (var neighbour in node.neighbours)
            {
                if (!neighbour.data.IsSolutionNode)
                    return false;
            }

            if (node.neighbours.Count != 2)
                return false;
        }

        return true;
    }

    private void DFS(Node<StarData> currentNode, HashSet<Node<StarData>> visited)
    {
        visited.Add(currentNode);

        foreach (var neighbour in currentNode.neighbours)
        {
            if (neighbour.data.IsSolutionNode && !visited.Contains(neighbour))
            {
                DFS(neighbour, visited);
            }
        }
    }
}
