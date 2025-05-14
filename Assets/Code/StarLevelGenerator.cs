using System.Collections.Generic;
using UnityEngine;


public class StarLevelGenerator : MonoBehaviour
{
    // Max number of stars per level (solution + filler stars)
    private const int NUM_STARS = 15;
    // Max distance from center for star placement
    private const float RADIUS = 10f;
    // Min distance between any two stars
    private const float MIN_DIST = 2f;
    // Size multiplier for shape generation
    private const float SHAPE_SIZE = 3f;


    public Graph<StarData> GenerateLevel(int level)
    {
        Graph<StarData> starGraph = new Graph<StarData>();
        List<Node<StarData>> solutionNodes = CreateSolutionShape(starGraph, level);
        CreateFillerStars(starGraph, solutionNodes);

        return starGraph;
    }

    // Selects the shape to be generated based on the current level
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
                IsSolutionNode = true
            });
            solutionNodes.Add(node);
        }

        return solutionNodes;
    }

    // Generates filler stars surrounding the solution nodes
    // Avoids positions where solution nodes were already pre placed
    private void CreateFillerStars(Graph<StarData> graph, List<Node<StarData>> avoidNodes)
    {
        var avoidPositions = new List<Vector3>();
        int numFillerStars = NUM_STARS - avoidNodes.Count;

        foreach (var node in avoidNodes)
            avoidPositions.Add(node.position);

        var fillerPositions = GenerateSpacedPositions(numFillerStars, avoidPositions
        );

        foreach (var pos in fillerPositions)
        {
            graph.AddNode(pos, new StarData
            {
                IsSolutionNode = false
            });
        }
    }

    // Transforms the hardcoded specified library shape via:
    // scale, rotation and offset
    // offset is a random pos within the radius from centre of screen (throws a dart on the dart board)
    public List<Vector3> GenerateGameplayShape(ShapeType type, bool applyRotation)
    {
        StarShapeData shape = ShapeLibrary.GetShape(type);
        float rotation;
        const float SAFE_ZONE_RADIUS = 9f;

        if (applyRotation)
            rotation = Random.Range(0f, 360f); 
        else
            rotation = 0f;

        Vector2 offset = Random.insideUnitCircle * SAFE_ZONE_RADIUS;

        return shape.GetTransformedVertices(SHAPE_SIZE, offset, rotation);
    }

    // Uses a simplified blue noise algorithm to distribute stars with minimum separation
    // Space isn't a constraint for the task it's doing
    // If you increase a level to a large number of stars it will run into constraints
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

    // Ensures the new star poisitons don't overlap with existing star positons
    // bruteforce checks distance to all placed filler stars and soltuion stars
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