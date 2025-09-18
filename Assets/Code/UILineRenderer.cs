using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILineRenderer : MaskableGraphic
{
    public Vector2[] points;

    public float lineWidth = 1.0f;
    public bool centre = true;

    [Tooltip("Connect the last point to the first point to close the shape.")]
    public bool loop = false;

    

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        if (points == null || points.Length < 2)
        {
            return;
        }

        Vector2 offset = centre ? (rectTransform.sizeDelta / 2f) : Vector2.zero;

        List<Vector2> drawPoints = new List<Vector2>(points);

        // Connect the last point to the first point to close the shape
        if (loop && drawPoints.Count > 1)
        {
            drawPoints.Add(drawPoints[0]); 
        }


        for (int i = 0; i < drawPoints.Count - 1; i++)
        {
            Vector2 startPoint = drawPoints[i];
            Vector2 endPoint = drawPoints[i + 1];


            // get the direction vector of the line and rotate vector 90 degrees * by the half of the lines width
            Vector2 lineDirection = (endPoint - startPoint).normalized;
            Vector2 perpendicularOffset = new Vector2(-lineDirection.y, lineDirection.x) * (lineWidth / 2);

            // Create the 4 vertices for the quad
            UIVertex[] quad = new UIVertex[4];
            quad[0] = CreateVertex((startPoint - perpendicularOffset), color);
            quad[1] = CreateVertex((startPoint + perpendicularOffset), color);
            quad[2] = CreateVertex((endPoint + perpendicularOffset) , color);
            quad[3] = CreateVertex((endPoint - perpendicularOffset), color);



            vh.AddUIVertexQuad(quad);
        }
    }

    private UIVertex CreateVertex(Vector2 position, Color32 vertexColor)
    {
        UIVertex vertex = UIVertex.simpleVert;
        vertex.position = position;
        vertex.color = vertexColor;
        return vertex;
    }
}
