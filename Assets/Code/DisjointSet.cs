using System.Collections.Generic;
using UnityEngine;

public class DisjointSet
{
    private Dictionary<Vector2Int, Vector2Int> parent = new();

    public void MakeSet(Vector2Int pos)
    {
        if (!parent.ContainsKey(pos))
            parent[pos] = pos;
    }

    public Vector2Int Find(Vector2Int pos)
    {
        if (!parent.ContainsKey(pos)) MakeSet(pos);

        if (!parent[pos].Equals(pos))
            parent[pos] = Find(parent[pos]);

        return parent[pos];
    }

    public void Union(Vector2Int a, Vector2Int b)
    {
        Vector2Int rootA = Find(a);
        Vector2Int rootB = Find(b);
        if (!rootA.Equals(rootB))
            parent[rootB] = rootA;
    }

    public bool Connected(Vector2Int a, Vector2Int b)
    {
        return Find(a).Equals(Find(b));
    }
}
