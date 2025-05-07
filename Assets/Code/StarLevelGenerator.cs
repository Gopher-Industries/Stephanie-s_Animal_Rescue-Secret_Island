using System.Collections.Generic;
using UnityEngine;

public class StarLevelGenerator : MonoBehaviour
{
    private const int NUM_STARS = 9;
    private const float RADIUS = 10f;
    private const float MIN_DIST = 2f;


    public Graph<StarData> GenerateLevel()
    {
        Graph<StarData> starGraph = new Graph<StarData>();
        List<Node<StarData>> solutionNodes = CreateSolutionShape(starGraph);
        CreateFillerStars(starGraph, solutionNodes);

        return starGraph;
    }

    private List<Node<StarData>> CreateSolutionShape(Graph<StarData> starGraph)
    {
        var trianglePoints = GenerateTriangle();
        var solutionNodes = new List<Node<StarData>>();

        foreach (var pos in trianglePoints)
        {
            var node = starGraph.AddNode(pos, new StarData
            {
                IsSolutionNode = true,
                IsSelected = false
            });
            solutionNodes.Add(node);
        }

        return solutionNodes;
    }

    private void CreateFillerStars(Graph<StarData> graph, List<Node<StarData>> avoidNodes)
    {
        var avoidPositions = new List<Vector3>();

        foreach (var node in avoidNodes)
            avoidPositions.Add(node.position);


        var fillerPositions = GenerateSpacedPositions(
            NUM_STARS - avoidNodes.Count,
            avoidPositions
        );

        foreach (var pos in fillerPositions)
        {
            graph.AddNode(pos, new StarData
            {
                IsSolutionNode = false,
                IsSelected = false
            });
        }
    }

    private List<Vector3> GenerateTriangle()
    {
        float sideLength = 3f;
        float height = Mathf.Sqrt(3f) / 2f * sideLength;


        Vector3 p1 = new Vector3(0, height / 2f, 0);
        Vector3 p2 = new Vector3(-sideLength / 2f, -height / 2f, 0);
        Vector3 p3 = new Vector3(sideLength / 2f, -height / 2f, 0);


        float angle = Random.Range(0f, 360f);
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        Vector2 offset = Random.insideUnitCircle * (RADIUS * 0.5f);


        List<Vector3> result = new List<Vector3>
    {
        rotation * p1 + (Vector3)offset,
        rotation * p2 + (Vector3)offset,
        rotation * p3 + (Vector3)offset
    };

        return result;
    }

    // Blue noise without even distribution
    private List<Vector3> GenerateSpacedPositions(int count, List<Vector3> avoidPoints)
    {
        List<Vector3> result = new List<Vector3>();
        int attempts = 0;
        int maxAttempts = 500;

        while (result.Count < count && attempts < maxAttempts)
        {
            Vector3 candidate = Random.insideUnitCircle * RADIUS;

            if (IsPositionValid(candidate, result, avoidPoints))
                result.Add(candidate);

            attempts++;
        }

        return result;
    }

    // Check via euclidean distance
    private bool IsPositionValid(Vector3 candidate, List<Vector3> existing, List<Vector3> avoid)
    {
        foreach (var pos in existing)
            if (Vector3.Distance(pos, candidate) < MIN_DIST)
                return false;

        foreach (var pos in avoid)
            if (Vector3.Distance(pos, candidate) < MIN_DIST)
                return false;

        return true;
    }
}