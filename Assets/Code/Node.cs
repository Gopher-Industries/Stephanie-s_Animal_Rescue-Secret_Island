using System.Collections.Generic;
using UnityEngine;


public class Node<T>
{
    // The id of this star node | increases by 1 each time,
    // The position of this star node,
    // The list of neighbouring nodes directly connected to this node,
    // Generic data to be passed in
    public int id { get; }
    public Vector3 position { get; private set; }
    public List<Node<T>> neighbours { get; private set; }
    public T data { get; private set; }


    public Node(int nodeId, Vector3 pos, T nodeData)
    {
        id = nodeId;
        position = pos;
        data = nodeData;
        neighbours = new List<Node<T>>();
    }

    public void AddNeighbour(Node<T> neighbor)
    {
        if (!neighbours.Contains(neighbor))
        {
            neighbours.Add(neighbor);
        }

    }
    
    public void RemoveNeighbour(Node<T> neighbor)
    {
        if (neighbours.Contains(neighbor))
        {
            neighbours.Remove(neighbor);
        }
    }

    public void SetPosition(Vector3 newPosition)
    {
        position = newPosition;
    }

}
