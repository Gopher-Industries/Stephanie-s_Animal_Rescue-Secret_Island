using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StarUIManager : MonoBehaviour
{
    [Header("Preview Canvas")]
    [SerializeField] private Canvas worldSpaceCanvas;
    [SerializeField] private TextMeshProUGUI shapeLabel;

    private Vector2 previewAnchor = new Vector2(-21.5f, -9f);
    private Color lineColor = Color.yellow;
    private Color cubeColor = Color.yellow;
    private LineRenderer lineRenderer;
    private Camera mainCamera;

    private const float PREVIEW_SCALE = 3f;
    private const float CUBE_SIZE = 0.5f;
    private const float LINE_WIDTH = 0.25f;

    private List<GameObject> cubeNodes = new List<GameObject>();

    void Awake()
    {
        mainCamera = Camera.main;
        if (lineRenderer == null)
        {
            CreateLineRenderer();
        }

        if (worldSpaceCanvas == null)
        {
            worldSpaceCanvas = GetComponent<Canvas>();
        }
    }

    // LineRenderer for preview shape
    private void CreateLineRenderer()
    {
        GameObject lrObject = new GameObject("ShapePreviewLines");
        lineRenderer = lrObject.AddComponent<LineRenderer>();

        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = 0;
        lineRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;  
        lineRenderer.startWidth = LINE_WIDTH;
        lineRenderer.endWidth = LINE_WIDTH;
        lrObject.transform.position = new Vector3(0, 0, 0);
    }

    // displays a visual preview of the given shape type in the corner of the screen
    public void ShowPreview(ShapeType shapeType)
    {
        if (lineRenderer == null)
        {
            CreateLineRenderer();
        }

        ClearPreviousPreview();

        float orthoScale = Camera.main.orthographicSize / 11f;
        worldSpaceCanvas.transform.localScale = Vector3.one * 0.05f * orthoScale;

        shapeLabel.text = shapeType.ToString();
        shapeLabel.fontSize = Mathf.RoundToInt(36 * orthoScale);

        StarShapeData shape = ShapeLibrary.GetShape(shapeType);
        Vector3[] vertices = CalculatePreviewVertices(shape.normalisedVertices);

        lineRenderer.positionCount = vertices.Length + 1;
        lineRenderer.SetPositions(vertices);
        lineRenderer.SetPosition(vertices.Length, vertices[0]);


        foreach (Vector3 vertex in vertices)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.localPosition = new Vector3(vertex.x, vertex.y, 0);
            cube.transform.localScale = Vector3.one * CUBE_SIZE;


            Material cubeMat = new Material(Shader.Find("Unlit/Color"));
            cubeMat.color = cubeColor;
            cube.GetComponent<Renderer>().material = cubeMat;

            Destroy(cube.GetComponent<BoxCollider>());

            cubeNodes.Add(cube);
        }
    }

    // Converts normalised shape positions to world space positons anchored to the previewAnchor
    private Vector3[] CalculatePreviewVertices(Vector2[] normalisedVertices)
    {
        Vector3[] vertices = new Vector3[normalisedVertices.Length];
        for (int i = 0; i < normalisedVertices.Length; i++)
        {
            vertices[i] = new Vector3(
                previewAnchor.x + normalisedVertices[i].x * PREVIEW_SCALE,
                previewAnchor.y + normalisedVertices[i].y * PREVIEW_SCALE,
                0
            );
        }
        return vertices;
    }

    // Clears the shape preview
    private void ClearPreviousPreview()
    {
        lineRenderer.positionCount = 0;

        foreach (GameObject cube in cubeNodes)
        {
            Destroy(cube);
        }
        cubeNodes.Clear();
    }

    public void HidePreview()
    {
        ClearPreviousPreview();
    }
}
