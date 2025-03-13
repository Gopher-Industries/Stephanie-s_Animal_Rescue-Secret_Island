using System.Collections.Generic;
using UnityEngine;

public class NumberLinksLevel3 : MonoBehaviour
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
    public Material material5; // Material 5
    public Material material6; // Material 6


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
        // Adjusted for the 6 pairs (minus 1 for all coordinates)
        if ((x == 0 && y == 5) || (x == 0 && y == 0)) // Pair (1,6) and (1,1)
        {
            return material1;
        }
        else if ((x == 0 && y == 4) || (x == 0 && y == 2)) // Pair (1,5) and (1,3)
        {
            return material2;
        }
        else if ((x == 1 && y == 0) || (x == 5 && y == 2)) // Pair (2,1) and (6,3)
        {
            return material3;
        }
        else if ((x == 2 && y == 1) || (x == 5 && y == 4)) // Pair (3,2) and (6,5)
        {
            return material4;
        }
        else if ((x == 2 && y == 2) || (x == 5 && y == 5)) // Pair (3,3) and (6,6)
        {
            return material5;
        }
        else if ((x == 2 && y == 3) || (x == 3 && y == 5)) // Pair (3,4) and (4,6)
        {
            return material6;
        }
        else
        {
            return null; // Invalid cell
        }
    }


    private bool IsValidStartingCell(int x, int y)
    {
        return DetermineMaterial(x, y) != null;
    }

    private bool IsValidEndingCell(int startX, int startY, int endX, int endY)
    {
        // Adjusted for the 6 pairs (minus 1 for all coordinates)
        if ((startX == 0 && startY == 5 && endX == 0 && endY == 0) || (startX == 0 && startY == 0 && endX == 0 && endY == 5)) // Pair (1,6) and (1,1)
            return true;
        if ((startX == 0 && startY == 4 && endX == 0 && endY == 2) || (startX == 0 && startY == 2 && endX == 0 && endY == 4)) // Pair (1,5) and (1,3)
            return true;
        if ((startX == 1 && startY == 0 && endX == 5 && endY == 2) || (startX == 5 && startY == 2 && endX == 1 && endY == 0)) // Pair (2,1) and (6,3)
            return true;
        if ((startX == 2 && startY == 1 && endX == 5 && endY == 4) || (startX == 5 && startY == 4 && endX == 2 && endY == 1)) // Pair (3,2) and (6,5)
            return true;
        if ((startX == 2 && startY == 2 && endX == 5 && endY == 5) || (startX == 5 && startY == 5 && endX == 2 && endY == 2)) // Pair (3,3) and (6,6)
            return true;
        if ((startX == 2 && startY == 3 && endX == 3 && endY == 5) || (startX == 3 && startY == 5 && endX == 2 && endY == 3)) // Pair (3,4) and (4,6)
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
