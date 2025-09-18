using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StarUIManager : MonoBehaviour
{
    [Header("Level Complete Panel")]
    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private TextMeshProUGUI levelCompleteText;

    [Header("Game Over Panel")]
    [SerializeField] private GameObject gameCompletePanel;

    [Header("Shape Preview Panel")]
    [SerializeField] private GameObject shapePreviewPanel;
    [SerializeField] private UILineRenderer shapePreview1;
    [SerializeField] private UILineRenderer shapePreview2;
    [SerializeField] private GameObject previewNodePrefab;

    [Header("Buttons")]
    [SerializeField] private GameObject showPreviewButton;

    private List<GameObject> activePreviewNodes = new List<GameObject>();


    void Awake()
    {
        if (shapePreviewPanel != null)
        {
            shapePreviewPanel.SetActive(false);
        }
    }

    public void LevelCompleteShow(int level)
    {
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
            if (levelCompleteText != null)
            {
                levelCompleteText.text = $"Level {level} Complete!";
            }
        }
    }

    public void LevelCompleteHide()
    {
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(false);
        }
    }

    public void ShowGameCompleteScreen()
    {
        if (gameCompletePanel != null)
        {
            gameCompletePanel.SetActive(true);
        }
    }

    public void ToggleShapePreview()
    {
        if (shapePreviewPanel != null)
        {
            shapePreviewPanel.SetActive(!shapePreviewPanel.activeSelf);
        }
    }

    public void DisplayShapePreviews(List<(ShapeType type, bool rotated)> shapes)
    {
        if (shapePreviewPanel == null) return;
        // clear old nodes
        ClearPreviewNodes();

        // Hide both previews initially
        shapePreview1.gameObject.SetActive(false);
        shapePreview2.gameObject.SetActive(false);

        // Configure the first shape preview
        if (shapes.Count > 0)
        {
            ConfigurePreview(shapePreview1, shapes[0].type);
        }

        // Configure the second shape preview if it exists
        if (shapes.Count > 1)
        {
            ConfigurePreview(shapePreview2, shapes[1].type);
        }
    }

    private void ConfigurePreview(UILineRenderer previewRenderer, ShapeType shapeType)
    {
        // Get the container's RectTransform
        RectTransform containerRect = previewRenderer.GetComponent<RectTransform>();
        if (containerRect == null) return;

        // Determine the available space (use the smaller of width or height)
        float fitSize = Mathf.Min(containerRect.rect.width, containerRect.rect.height);

        // Calculate the scale with some padding so it's not touching the edges
        // 90% of the container size
        float padding = 0.9f; 
        float scale = fitSize * padding;

        StarShapeData shapeData = ShapeLibrary.GetShape(shapeType); 
        Vector2[] scaledPoints = new Vector2[shapeData.normalisedVertices.Length]; 

        for (int i = 0; i < shapeData.normalisedVertices.Length; i++)
        {
            // Apply scale based on container size
            scaledPoints[i] = shapeData.normalisedVertices[i] * scale; 
        }

        previewRenderer.points = scaledPoints;
        previewRenderer.gameObject.SetActive(true);

        previewRenderer.SetVerticesDirty();

        if (previewNodePrefab != null)
        {
            foreach (Vector2 point in scaledPoints)
            {
                GameObject nodeInstance = Instantiate(previewNodePrefab, previewRenderer.transform);
                nodeInstance.GetComponent<RectTransform>().anchoredPosition = point;
                activePreviewNodes.Add(nodeInstance);
            }
        }
    }

    public void SetPreviewButtonVisibility(bool isVisible)
    {
        if (showPreviewButton != null)
        {
            showPreviewButton.SetActive(isVisible);
        }
    }

    private void ClearPreviewNodes()
    {
        foreach (GameObject node in activePreviewNodes)
        {
            Destroy(node);
        }
        activePreviewNodes.Clear();
    }
}
