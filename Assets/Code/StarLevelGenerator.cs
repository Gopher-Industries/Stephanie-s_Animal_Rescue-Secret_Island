using System.Collections.Generic;
using System.Linq;
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
        List<Node<StarData>> solutionNodes = CreateSolutionShapes(starGraph, level);
        CreateFillerStars(starGraph, solutionNodes);

        return starGraph;
    }

    private List<Node<StarData>> CreateSolutionShapes(Graph<StarData> starGraph, int level)
    {
        List<Node<StarData>> allSolutionNodes = new();
        List<Vector3> occupiedPositions = new();

        // Hardcoded shapes for each level
        Dictionary<int, List<(ShapeType type, bool rotated)>> levelShapes = new()
    {
        { 1, new() { (ShapeType.Triangle, false) } },
        { 2, new() { (ShapeType.Triangle, true) } },
        { 3, new() { (ShapeType.Square, false) } },
        { 4, new() { (ShapeType.Square, true) } },
        { 5, new() { (ShapeType.Triangle, false), (ShapeType.Square, false) } },
        { 6, new() { (ShapeType.Triangle, true), (ShapeType.Square, true) } }
        // Add more levels as needed
    };

        if (!levelShapes.TryGetValue(level, out var shapesForLevel))
        {
            shapesForLevel = new() { (ShapeType.Triangle, false) }; // TODO: this should transition to exit game
        }

        foreach (var (shapeType, applyRotation) in shapesForLevel)
        {
            var shapePoints = GenerateNonOverlappingShape(shapeType, applyRotation, occupiedPositions);

            foreach (var pos in shapePoints)
            {
                var node = starGraph.AddNode(pos, new StarData
                {
                    IsSolutionNode = true
                });

                allSolutionNodes.Add(node);
                occupiedPositions.Add(pos);
            }
        }

        return allSolutionNodes;
    }

    // ensures the shapes don't overlap
    private List<Vector3> GenerateNonOverlappingShape(ShapeType shapeType, bool applyRotation, List<Vector3> occupiedPositions)
    {
        List<Vector3> newShape;
        int maxAttempts = 50;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            newShape = GenerateGameplayShape(shapeType, applyRotation);

            bool isFarEnough = newShape.All(p =>
                occupiedPositions.All(existing =>
                    Vector3.Distance(existing, p) >= MIN_DIST));

            if (isFarEnough)
                return newShape;
        }
        Debug.Log("If you see this GenerateNonOverlappingShape in the Level Generator somehow to spawn multiple shapes spread out. Even though at this point it only has to avoid other shapes and not filler stars");
        // just return the shape anyway we're cooked if this happens
        return GenerateGameplayShape(shapeType, applyRotation);
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