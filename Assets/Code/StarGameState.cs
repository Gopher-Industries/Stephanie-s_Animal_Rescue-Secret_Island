using System.Collections.Generic;
using UnityEngine;

public class StarGameState
{
    public Graph<StarData> currentGraph { get; private set; }
    public List<Node<StarData>> solutionNodes { get; }
    public bool isSolutionValid { get; private set; }

    public StarGameState()
    {
        solutionNodes = new List<Node<StarData>>();
    }

    public void Initialize(Graph<StarData> graph)
    {
        currentGraph = graph;
        solutionNodes.Clear();
        ValidateSolution();
    }

    public bool TryAddConnection(int nodeAId, int nodeBId)
    {
        var nodeA = FindNode(nodeAId);
        var nodeB = FindNode(nodeBId);

        if (nodeA == null || nodeB == null) return false;

        currentGraph.ConnectNodes(nodeA, nodeB);

        ValidateSolution();
        return true;
    }

    public bool TryRemoveConnection(int nodeAId, int nodeBId)
    {
        var nodeA = FindNode(nodeAId);
        var nodeB = FindNode(nodeBId);

        if (nodeA == null || nodeB == null) return false;

        currentGraph.DisconnectNodes(nodeA, nodeB);

        ValidateSolution();
        return true;
    }

    public bool ToggleNodeSelection(int nodeId, bool selected)
    {
        Node<StarData> node = FindNode(nodeId);

        if (node == null)
        {
            Debug.LogWarning($"Node {nodeId} not found!");
            return false;
        }

        node.data.IsSelected = selected;
        return true;
    }

    private Node<StarData> FindNode(int id)
    {
        foreach (var node in currentGraph.Nodes)
        {
            if (node.id == id) return node;
        }
        return null;
    }

    public Node<StarData> GetNodeById(int nodeId)
    {
        foreach (var node in currentGraph.Nodes)
        {
            if (node.id == nodeId) return node;
        }
        return null;
    }

    private void ValidateSolution()
    {
        isSolutionValid = CheckSolution(solutionNodes);
    }

    private bool CheckSolution(List<Node<StarData>> solutionNodes)
    {
        if (solutionNodes.Count < 2)
            return false;

        HashSet<Node<StarData>> visited = new HashSet<Node<StarData>>();
        DFS(solutionNodes[0], visited);


        if (visited.Count != solutionNodes.Count)
            return false;


        foreach (var node in solutionNodes)
        {
            int solutionNeighbors = 0;
            foreach (var neighbour in node.neighbours)
            {
                if (neighbour.data.IsSolutionNode)
                    solutionNeighbors++;
                else
                    return false;
            }

            if (solutionNeighbors != 2)
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
