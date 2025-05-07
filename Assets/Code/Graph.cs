using System.Collections.Generic;
using UnityEngine;


public class Graph<T>
{
    // Private, internal list that holds all nodes on the graph
    private List<Node<T>> nodes = new List<Node<T>>();

    // Public, read-only access to the list of nodes
    public IReadOnlyList<Node<T>> Nodes => nodes;

    // This property represents the number of nodes in the graph
    public int Count => nodes.Count;
    // Each graph maintains its own ID counter
    private int nextNodeId;


    public Node<T> AddNode(Vector3 position, T data)
    {
        Node<T> node = new Node<T>(nextNodeId++, position, data);
        nodes.Add(node);
        return node;
    }

    public void RemoveNode()
    {
        //TODO: Implement this if you require the use of this graph & node class outside of the constellation minigame
    }

    public void ConnectNodes(Node<T> a, Node<T> b)
    {
        if (!nodes.Contains(a) || !nodes.Contains(b))
            Debug.Log("Attempted to connect nodes not in the graph");

        a.AddNeighbour(b);
        b.AddNeighbour(a);
    }

    public void DisconnectNodes(Node<T> a, Node<T> b)
    {
        if (!nodes.Contains(a) || !nodes.Contains(b))
            Debug.Log("Attempted to connect nodes not in the graph");

        a.RemoveNeighbour(b);
        b.RemoveNeighbour(a);
    }

    public void Clear()
    {
        nodes.Clear();
        nextNodeId = 0;
    }
}
