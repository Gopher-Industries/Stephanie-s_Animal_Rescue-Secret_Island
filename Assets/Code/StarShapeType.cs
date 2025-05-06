using System.Collections.Generic;
using UnityEngine;

public enum ShapeType { Square, Triangle}

[System.Serializable]
public class StarShapeData
{
    public ShapeType type;
    public Vector2[] normalizedVertices; 
    public bool isClosedShape = true;

    
    public List<Vector3> GetTransformedVertices(float size, Vector2 offset, float rotationDegrees = 0f)
    {
        Quaternion rotation = Quaternion.Euler(0, 0, rotationDegrees);
        List<Vector3> vertices = new List<Vector3>(normalizedVertices.Length);

        foreach (Vector2 vert in normalizedVertices)
        {
            vertices.Add(rotation * (vert * size) + (Vector3)offset);
        }
        return vertices;
    }

    
    public List<Vector3> GetPreviewVertices(float size, Vector2 screenAnchor)
    {
        return GetTransformedVertices(
            size: size,
            offset: screenAnchor,
            rotationDegrees: 0f 
        );
    }
}