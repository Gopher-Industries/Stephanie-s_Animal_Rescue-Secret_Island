using System.Collections.Generic;
using UnityEngine;

public class StarLevelGenerator : MonoBehaviour
{
    private const int NUM_STARS = 9;
    private const float RADIUS = 10f;
    private const float MIN_DIST = 2f;
    private const float SHAPE_SIZE = 3f;


    public Graph<StarData> GenerateLevel(int level)
    {
        Graph<StarData> starGraph = new Graph<StarData>();
        List<Node<StarData>> solutionNodes = CreateSolutionShape(starGraph, level);
        CreateFillerStars(starGraph, solutionNodes);

        return starGraph;
    }

    private List<Node<StarData>> CreateSolutionShape(Graph<StarData> starGraph, int level)
    {
        ShapeType shapeType;
        bool applyRotation;
        switch (level)
        {
            case 1: 
                shapeType = ShapeType.Triangle;
                applyRotation = false;
                break;
            case 2: 
                shapeType = ShapeType.Triangle;
                applyRotation = true;
                break;
            case 3: 
                shapeType = ShapeType.Square;
                applyRotation = false;
                break;
            case 4: 
                shapeType = ShapeType.Square;
                applyRotation = true;
                break;
            default:
                shapeType = ShapeType.Triangle;
                applyRotation = false;
                break;
        }

        var solutionShapePoints = GenerateGameplayShape(shapeType, applyRotation);
        var solutionNodes = new List<Node<StarData>>();

        foreach (var pos in solutionShapePoints)
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

    public List<Vector3> GenerateGameplayShape(ShapeType type,bool applyRotation)
    {
        StarShapeData shape = ShapeLibrary.GetShape(type);
        float rotation;

        if (applyRotation)
            rotation = Random.Range(0f, 360f); 
        else
            rotation = 0f;


        Vector2 offset = Random.insideUnitCircle * 5f;

        return shape.GetTransformedVertices(SHAPE_SIZE, offset, rotation);
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