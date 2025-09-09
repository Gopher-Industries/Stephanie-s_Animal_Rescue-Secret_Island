using UnityEngine;
using System.Collections.Generic;

public static class ShapeLibrary
{
    // A static dictionary that maps ShapeType to it's corresponding hard coded normalised shape data
    private static readonly Dictionary<ShapeType, StarShapeData> shapes = new Dictionary<ShapeType, StarShapeData>()
    {
        {
            ShapeType.Square,
            new StarShapeData()
            {
                type = ShapeType.Square,
                normalisedVertices = new Vector2[]
                {
                    new Vector2(-0.5f, 0.5f),  
                    new Vector2(0.5f, 0.5f),   
                    new Vector2(0.5f, -0.5f),  
                    new Vector2(-0.5f, -0.5f)  
                }
            }
        },
        {
            ShapeType.Triangle,
            new StarShapeData()
            {
                type = ShapeType.Triangle,
                normalisedVertices = new Vector2[]
                {
                    new Vector2(0f, 0.5f),
                    new Vector2(-0.577f, -0.5f),
                    new Vector2(0.577f, -0.5f)
                }
            }
        },
        {
            ShapeType.Pentagon,
            new StarShapeData()
            {
                type = ShapeType.Pentagon,
                normalisedVertices = new Vector2[]
                {
                    new Vector2( 0f,  0.851f),
                    new Vector2( 0.809f,  0.263f),
                    new Vector2( 0.5f, -0.688f),
                    new Vector2(-0.5f, -0.688f),
                    new Vector2(-0.809f,  0.263f)

                }
            }
        },
        {
            ShapeType.Hexagon,
            new StarShapeData()
            {
                type = ShapeType.Hexagon,
                normalisedVertices = new Vector2[]
                {
                    new Vector2( 1f,  0),
                    new Vector2( 0.5f,  0.866f),
                    new Vector2(-0.5f,  0.866f),
                    new Vector2(-1f,  0f),
                    new Vector2(-0.5f, -0.866f),
                    new Vector2( 0.5f, -0.866f)
                }
            }
        }
    };

    // gets the shape data associated with a specific shape type
    public static StarShapeData GetShape(ShapeType type) => shapes[type];
}