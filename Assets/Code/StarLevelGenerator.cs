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

    // Generates the graph and caches solution edges
    // TODO: Refactor when you figure out a better level system.
    public (Graph<StarData>, List<(int, int)>) GenerateLevel(int level)
    {
        Graph<StarData> starGraph = new Graph<StarData>();
        List<(int, int)> solutionEdges = new List<(int, int)>();
        List<Node<StarData>> solutionNodes = new List<Node<StarData>>();


        (solutionNodes, solutionEdges) = CreateSolutionShapes(starGraph, level);
        CreateFillerStars(starGraph, solutionNodes);

        return (starGraph, solutionEdges);
    }
    
    // TODO: Refactor when you figure out a better level system as this function became extremely bloated
    // TODO: Increase/decrease filler stars based on amount of shapes
    private (List<Node<StarData>>, List<(int, int)>) CreateSolutionShapes(Graph<StarData> starGraph, int level)
    {
        List<Node<StarData>> allSolutionNodes = new();
        List<Vector3> occupiedPositions = new();
        List<(int, int)> solutionEdges = new List<(int, int)>();

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

        // Triggers when game is out of levels
        if (!levelShapes.TryGetValue(level, out var shapesForLevel))
        {
            shapesForLevel = new() { (ShapeType.Triangle, false) }; // TODO: this should transition to exit game
        }

        foreach (var (shapeType, applyRotation) in shapesForLevel)
        {
            var shapePoints = GenerateNonOverlappingShape(shapeType, applyRotation, occupiedPositions);
            List<Node<StarData>> shapeNodes = new();
            foreach (Vector3 position in shapePoints)
            {
                var node = starGraph.AddNode(position, new StarData { IsSolutionNode = true });
                shapeNodes.Add(node);
                allSolutionNodes.Add(node);
                occupiedPositions.Add(position);
            }

            // Cache edge data between consecutive shape nodes (including closing the loop)
            for (int i = 0; i < shapeNodes.Count; i++)
            {
                int nextIndex = (i + 1) % shapeNodes.Count;
                solutionEdges.Add((shapeNodes[i].id, shapeNodes[nextIndex].id));
            }
        }

        return (allSolutionNodes, solutionEdges);
    }

    // Ensures the shapes don't overlap by bruteforce
    // TODO: Might be better to set up a grid and place the shapes on a random tile
    private List<Vector3> GenerateNonOverlappingShape(ShapeType shapeType, bool applyRotation, List<Vector3> occupiedPositions)
    {
        List<Vector3> newShape;
        int maxAttempts = 50;

        // Check that every point in the new shape is far enough away from all existing occupied positions
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            newShape = GenerateGameplayShape(shapeType, applyRotation);

            bool isFarEnough = true;

            foreach (Vector3 newPoint in newShape)
            {
                foreach (Vector3 existing in occupiedPositions)
                {
                    if (Vector3.Distance(existing, newPoint) < MIN_DIST)
                    {
                        isFarEnough = false;
                        break;
                    }
                }

                if (!isFarEnough)
                    break;
            }

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
            graph.AddNode(pos, new StarData {IsSolutionNode = false});
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