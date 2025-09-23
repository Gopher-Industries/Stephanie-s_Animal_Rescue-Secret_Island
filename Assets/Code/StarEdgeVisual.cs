using UnityEngine;

public class StarEdgeVisual : MonoBehaviour
{
    private LineRenderer line;
    private PolygonCollider2D polygonCollider;
    private Material edgeMaterial;


    public void Initialize(Vector3 start, Vector3 end, float width)
    {
        line = GetComponent<LineRenderer>();
        polygonCollider = GetComponent<PolygonCollider2D>();
        edgeMaterial = new Material(line.material);
        line.material = edgeMaterial;
        edgeMaterial.EnableKeyword("_EMISSION");

        line.positionCount = 2;
        line.SetPositions(new Vector3[] { start, end });
        line.startWidth = width;
        line.endWidth = width;
        line.useWorldSpace = true;

        UpdateCollider(start, end, width);
    }

    // This video helped me form the correct shape for the LineRenderer collider box:
    // https://youtu.be/BfP0KyOxVWs?t=139
    // creates a rectangle collider hit box around the LineRenderer
    // start is x1,y1 and end is x2,y2
    // line direction is used to calculate the perpendicular of the line
    // the offset (half of the desired hitbox width) is applied along the perpendicular 
    // applying the offset on both ends of the line (above and below each end) we form the points for the box
    // setpath uses the 4 points (corners) of to draw the polygon collider box
    private void UpdateCollider(Vector3 start, Vector3 end, float width)
    {
        Vector2 lineDirection = (end - start).normalized;
        Vector2 perpendicularOffset = (width/2) * new Vector2(-lineDirection.y, lineDirection.x);

        Vector2[] points = new Vector2[4];
        points[0] = start + (Vector3)perpendicularOffset;
        points[1] = start - (Vector3)perpendicularOffset;
        points[2] = end - (Vector3)perpendicularOffset;
        points[3] = end + (Vector3)perpendicularOffset;

        polygonCollider.SetPath(0, points);
    }

    public void SetColour(Color colour)
    {
        if (edgeMaterial == null) 
            return;

        //line.startColor = colour;
        //line.endColor = colour;

        edgeMaterial.SetColor("_EmissionColor", colour * 2f);
    }
}