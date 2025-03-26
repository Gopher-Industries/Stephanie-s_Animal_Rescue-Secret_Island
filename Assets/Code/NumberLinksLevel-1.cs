using System.Collections.Generic;
using UnityEngine;

public class PathfindingGame : MonoBehaviour
{
    private GridClass grid;
    private bool isDragging; // Tracks whether the user is dragging
    private int startX = -1, startY = -1; // Starting cell coordinates
    private int endX = -1, endY = -1; // Ending cell coordinates
    private Material dragMaterial; // Material to use during the drag
    private HashSet<Vector2Int> currentDragCells; // Tracks cells affected during the current drag

    public Vector3 gridOriginPosition = new Vector3(0, 0, 0); // Origin of the grid
    public int gridWidth = 4; // Number of grid cells horizontally
    public int gridHeight = 4; // Number of grid cells vertically
    public float cellWidth = 2f; // Width of each cell
    public float cellHeight = 2f; // Height of each cell
    public float gridZPosition = -0.5f; // Adjustable Z position of the grid
    public Material material1; // Material 1
    public Material material2; // Material 2
    public Material material3; // Material 3
    public Material material4; // Material 4

    void Start()
    {
        // Initialize the grid
        grid = new GridClass(gridWidth, gridHeight, cellWidth, cellHeight, gridOriginPosition, gridZPosition);
        currentDragCells = new HashSet<Vector2Int>(); // Initialize the HashSet
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Start dragging
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0;

            grid.GetXY(mouseWorldPosition, out startX, out startY);

            if (IsValidStartingCell(startX, startY))
            {
                dragMaterial = DetermineMaterial(startX, startY);
                Debug.Log($"Started Dragging from cell ({startX}, {startY}) with material {dragMaterial.name}");
                isDragging = true;
                currentDragCells.Clear(); // Clear previously affected cells
            }
            else
            {
                Debug.Log($"Invalid starting cell ({startX}, {startY}). Dragging not started.");
                isDragging = false;
            }
        }

        if (Input.GetMouseButton(0) && isDragging) // While dragging
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0;

            int x, y;
            grid.GetXY(mouseWorldPosition, out x, out y);
            Vector2Int currentCell = new Vector2Int(x, y);

            if (x >= 0 && y >= 0 && x < gridWidth && y < gridHeight && !currentDragCells.Contains(currentCell))
            {
                grid.ToggleCellMaterial(x, y, dragMaterial); // Apply material only once
                currentDragCells.Add(currentCell); // Mark the cell as affected
                Debug.Log($"Material applied to cell ({x}, {y})");
            }
        }

        if (Input.GetMouseButtonUp(0)) // Stop dragging
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0;

            grid.GetXY(mouseWorldPosition, out endX, out endY);

            if (IsValidEndingCell(startX, startY, endX, endY))
            {
                Debug.Log($"Valid drag: Started at ({startX}, {startY}) and ended at ({endX}, {endY})");
            }
            else
            {
                Debug.Log($"Invalid drag: Started at ({startX}, {startY}) and ended at ({endX}, {endY}). Resetting current drag cells.");
                ResetCurrentDragCells(); // Reset only the cells affected during this drag
            }

            isDragging = false;
            startX = startY = endX = endY = -1; // Reset coordinates
            currentDragCells.Clear(); // Clear the affected cells for this drag
        }
    }

    private Material DetermineMaterial(int x, int y)
    {
        // Check specific cell coordinates for material assignment
        if ((x == 0 && y == 3) || (x == 3 && y == 3))
        {
            return material1; // Material 1
        }
        else if ((x == 0 && y == 2) || (x == 2 && y == 0))
        {
            return material2; // Material 2
        }
        else if ((x == 3 && y == 2) || (x == 1 && y == 1))
        {
            return material3; // Material 3
        }
        else if ((x == 2 && y == 1) || (x == 3 && y == 0))
        {
            return material4; // Material 4
        }
        else
        {
            return null;
        }
    }

    private bool IsValidStartingCell(int x, int y)
    {
        return DetermineMaterial(x, y) != null;
    }

    private bool IsValidEndingCell(int startX, int startY, int endX, int endY)
    {
        // Check if the start and end cells are in the same pair
        if ((startX == 0 && startY == 3 && endX == 3 && endY == 3) || (startX == 3 && startY == 3 && endX == 0 && endY == 3))
            return true;
        if ((startX == 0 && startY == 2 && endX == 2 && endY == 0) || (startX == 2 && startY == 0 && endX == 0 && endY == 2))
            return true;
        if ((startX == 3 && startY == 2 && endX == 1 && endY == 1) || (startX == 1 && startY == 1 && endX == 3 && endY == 2))
            return true;
        if ((startX == 2 && startY == 1 && endX == 3 && endY == 0) || (startX == 3 && startY == 0 && endX == 2 && endY == 1))
            return true;

        return false; // Not in the same pair
    }

    private void ResetCurrentDragCells()
    {
        foreach (Vector2Int cell in currentDragCells)
        {
            grid.ToggleCellMaterial(cell.x, cell.y, null); // Reset the material to default (null)
            Debug.Log($"Reset cell ({cell.x}, {cell.y}) to default state.");
        }
        currentDragCells.Clear(); // Clear the current drag set
    }
}
