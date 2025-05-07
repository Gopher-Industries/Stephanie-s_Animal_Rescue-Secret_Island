using UnityEngine;

public class GridClass
{
    private int width;
    private int height;
    private float cellWidth;
    private float cellHeight;
    private Vector3 originPosition;
    private float zPosition;
    private GameObject[,] gridParents;
    private GameObject[,] backgroundQuads;
    private SpriteRenderer[,] imageRenderers;

    public float imageWidth = 1f;
    public float imageHeight = 1f;

    public GridClass(int width, int height, float cellWidth, float cellHeight, Vector3 originPosition, float zPosition)
    {
        this.width = width;
        this.height = height;
        this.cellWidth = cellWidth;
        this.cellHeight = cellHeight;
        this.originPosition = originPosition;
        this.zPosition = zPosition;

        gridParents = new GameObject[width, height];
        backgroundQuads = new GameObject[width, height];
        imageRenderers = new SpriteRenderer[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                CreateCell(x, y);
            }
        }
    }

    private void CreateCell(int x, int y)
    {
        Vector3 cellPosition = GetWorldPosition(x, y) + new Vector3(cellWidth, cellHeight) * 0.5f;

        GameObject parent = new GameObject($"Cell ({x},{y})");
        parent.transform.position = new Vector3(cellPosition.x, cellPosition.y, zPosition);

        // Create background quad (for dragging colour)
        GameObject background = GameObject.CreatePrimitive(PrimitiveType.Quad);
        background.transform.SetParent(parent.transform);
        background.transform.localPosition = Vector3.zero;
        background.transform.localScale = new Vector3(cellWidth, cellHeight, 1);
        Renderer bgRenderer = background.GetComponent<Renderer>();
        bgRenderer.material = new Material(Shader.Find("Unlit/Color"));
        bgRenderer.material.color = new Color(1, 1, 1, 1); // White, will be hidden
        bgRenderer.enabled = false; // Hide at start
        bgRenderer.sortingOrder = 5;
        backgroundQuads[x, y] = background;

        // Create image object (SpriteRenderer)
        GameObject imageObj = new GameObject("Image");
        imageObj.transform.SetParent(parent.transform);
        imageObj.transform.localPosition = new Vector3(0, 0, -0.01f);
        SpriteRenderer imageRenderer = imageObj.AddComponent<SpriteRenderer>();
        imageRenderer.sortingOrder = 10; // Above background
        imageObj.transform.localScale = new Vector3(imageWidth, imageHeight, 1f);

        imageRenderers[x, y] = imageRenderer;

        gridParents[x, y] = parent;
    }

    private Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x * cellWidth, y * cellHeight) + originPosition;
    }

    private bool IsValidPosition(int x, int y)
    {
        return x >= 0 && y >= 0 && x < width && y < height;
    }

    public void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition.x - originPosition.x) / cellWidth);
        y = Mathf.FloorToInt((worldPosition.y - originPosition.y) / cellHeight);
    }

    public void SetCellTexture(int x, int y, Texture2D texture)
    {
        if (IsValidPosition(x, y))
        {
            SpriteRenderer renderer = imageRenderers[x, y];
            if (renderer != null)
            {
                if (texture != null)
                {
                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    renderer.sprite = sprite;
                    renderer.transform.localScale = new Vector3(imageWidth, imageHeight, 1f);
                }
                else
                {
                    renderer.sprite = null; // ✅ this avoids the crash
                }
            }
        }
    }


    public void ToggleCellMaterial(int x, int y, Material material)
    {
        if (IsValidPosition(x, y))
        {
            Renderer bgRenderer = backgroundQuads[x, y].GetComponent<Renderer>();
            if (bgRenderer != null)
            {
                if (material != null)
                {
                    bgRenderer.enabled = true;
                    bgRenderer.material = material;
                }
                else
                {
                    bgRenderer.enabled = false;
                }
            }
        }
    }

    public void ResetCell(int x, int y)
    {
        if (IsValidPosition(x, y))
        {
            Renderer bgRenderer = backgroundQuads[x, y].GetComponent<Renderer>();
            if (bgRenderer != null)
            {
                bgRenderer.enabled = false; // Hide again
            }
        }
    }
}
