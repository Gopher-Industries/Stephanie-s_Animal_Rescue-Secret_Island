using System.Collections.Generic;
using UnityEngine;

public enum ShapeType { 
    Square, 
    Triangle,
    Pentagon,
    Hexagon
}

[System.Serializable]
public class StarShapeData
{
    public ShapeType type;
    public Vector2[] normalisedVertices; 


    // Tranforms the shapes vertices via scaling, translating and rotating
    public List<Vector3> GetTransformedVertices(float size, Vector2 offset, float rotationDegrees = 0f)
    {
        Quaternion rotation = Quaternion.Euler(0, 0, rotationDegrees);
        List<Vector3> vertices = new List<Vector3>(normalisedVertices.Length);

        foreach (Vector2 vertex in normalisedVertices)
        {
            vertices.Add(rotation * (vertex * size) + (Vector3)offset);
        }
        return vertices;
    }

    // gets the preview vertices for the shape without rotation
    public List<Vector3> GetPreviewVertices(float size, Vector2 screenAnchor)
    {
        return GetTransformedVertices(
            size: size,
            offset: screenAnchor,
            rotationDegrees: 0f 
        );
    }
}