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

        line.positionCount = 2;
        line.SetPositions(new Vector3[] { start, end });
        line.startWidth = width;
        line.endWidth = width;
        line.useWorldSpace = true;

        UpdateCollider(start, end, width);
    }

    // This video helped me form the correct shape for the lr collision:
    // https://youtu.be/BfP0KyOxVWs?t=139
    private void UpdateCollider(Vector3 start, Vector3 end, float width)
    {
        Vector2 dir = (end - start).normalized;
        Vector2 perpendicular = 0.5f * width * new Vector2(-dir.y, dir.x);

        Vector2[] points = new Vector2[4];
        points[0] = start + (Vector3)perpendicular;
        points[1] = start - (Vector3)perpendicular;
        points[2] = end - (Vector3)perpendicular;
        points[3] = end + (Vector3)perpendicular;

        polygonCollider.SetPath(0, points);
    }

    public void SetColour(Color colour)
    {
        if (edgeMaterial == null) 
            return;

        line.startColor = colour;
        line.endColor = colour;
    }
}