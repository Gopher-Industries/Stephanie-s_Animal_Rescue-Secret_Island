using UnityEngine;

public class GridClass
{
    private int width;
    private int height;
    private float cellWidth;
    private float cellHeight;
    private Vector3 originPosition;
    private float zPosition; // Adjustable Z position
    private GameObject[,] gridVisuals;

    public GridClass(int width, int height, float cellWidth, float cellHeight, Vector3 originPosition, float zPosition)
    {
        this.width = width;
        this.height = height;
        this.cellWidth = cellWidth;
        this.cellHeight = cellHeight;
        this.originPosition = originPosition;
        this.zPosition = zPosition;

        gridVisuals = new GameObject[width, height];

        // Create visual grid cells
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 cellPosition = GetWorldPosition(x, y) + new Vector3(cellWidth, cellHeight) * 0.5f;
                GameObject cell = CreateCell(cellPosition, cellWidth, cellHeight);
                gridVisuals[x, y] = cell;
            }
        }
    }

    private GameObject CreateCell(Vector3 position, float width, float height)
    {
        GameObject cell = GameObject.CreatePrimitive(PrimitiveType.Quad);

        // Set the position of the cell
        cell.transform.position = new Vector3(position.x, position.y, zPosition);
        cell.transform.localScale = new Vector3(width, height, 1);

        // Initially, make the cell invisible by disabling its Renderer
        Renderer renderer = cell.GetComponent<Renderer>();
        renderer.enabled = false;

        // Set sorting order for proper rendering
        renderer.sortingOrder = 10;

        Debug.Log($"Created cell at {position} with Z = {zPosition}");
        return cell;
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x * cellWidth, y * cellHeight) + originPosition;
    }

    public void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        // Convert world position to grid coordinates
        x = Mathf.FloorToInt((worldPosition.x - originPosition.x) / cellWidth);
        y = Mathf.FloorToInt((worldPosition.y - originPosition.y) / cellHeight);
    }

    public void ToggleCellMaterial(int x, int y, Material material)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            Renderer renderer = gridVisuals[x, y].GetComponent<Renderer>();
            if (renderer != null)
            {
                if (material != null)
                {
                    renderer.enabled = true; // Show the cell
                    renderer.material = material; // Set the active material
                    Debug.Log($"Activated cell at ({x}, {y}) with material {material.name}");
                }
                else
                {
                    renderer.enabled = false; // Hide the cell (reset to default state)
                    Debug.Log($"Reset cell at ({x}, {y}) to default state.");
                }
            }
            else
            {
                Debug.LogError($"No Renderer found for cell at ({x}, {y})");
            }
        }
    }


    public void ResetAllCells()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Renderer renderer = gridVisuals[x, y].GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.enabled = false; // Reset to default (invisible)
                }
            }
        }
        Debug.Log("All cells reset to default state.");
    }

}
