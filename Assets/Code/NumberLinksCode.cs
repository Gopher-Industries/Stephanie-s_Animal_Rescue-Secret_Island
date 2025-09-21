using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class NumberLinksLevel : MonoBehaviour
{
    private GridClass grid;
    private bool isDragging;
    private int startX = -1, startY = -1;
    private int endX = -1, endY = -1;
    private Material dragMaterial;
    private HashSet<(Vector2Int, int)> currentDragCells;
    private HashSet<(Vector2Int, int)> visitedCells;
    private Dictionary<Vector2Int, bool> occupiedCells;
    private int currentId = -1;

    [Header("Grid Settings")]
    public Vector3 gridOriginPosition = new Vector3(0, 0, 0);
    public int gridWidth = 4;
    public int gridHeight = 4;
    public float cellWidth = 2f;
    public float cellHeight = 2f;
    public float gridZPosition = -0.5f;

    [Header("Image Settings")]
    public float imageWidth = 0.8f;
    public float imageHeight = 0.8f;

    [Header("Pair Settings")]
    public Texture2D[] pairImages;      // Images displayed on the grid
    public Material[] dragMaterials;    // Materials used during dragging

    private Dictionary<Vector2Int, int> cellToPairId;

    IEnumerator Start()
    {
        yield return null;

        grid = new GridClass(gridWidth, gridHeight, cellWidth, cellHeight, gridOriginPosition, gridZPosition);
        grid.imageWidth = imageWidth;
        grid.imageHeight = imageHeight;
        currentDragCells = new HashSet<(Vector2Int, int)>();
        visitedCells = new HashSet<(Vector2Int, int)>();
        cellToPairId = new Dictionary<Vector2Int, int>();
        occupiedCells = new Dictionary<Vector2Int, bool>();


        SetupLevel();
        visitedCells.Clear();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseDown();
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            HandleMouseDrag();
        }

        if (Input.GetMouseButtonUp(0))
        {
            HandleMouseUp();
        }

        if (AllPairsVisited())
        {
            Debug.Log("🎉 All pairs connected! Level Complete!");
            //TO-DO implement level complete logic
        }
    }

    // Returns true if every pairId in cellToPairId has at least one cell in visitedCells
    bool AllPairsVisited()
    {
        var allPairIds = cellToPairId.Values.Distinct();
        var visitedPairIds = visitedCells.Select(cell => cell.Item2).Distinct();
        return allPairIds.All(id => visitedPairIds.Contains(id));
    }
    private void HandleMouseDown()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0;

        grid.GetXY(mouseWorldPosition, out startX, out startY);

        if (IsValidStartingCell(startX, startY))
        {
            dragMaterial = DetermineDragMaterial(startX, startY);
            isDragging = true;
            currentDragCells.Clear();
        }
        else
        {
            startX = startY = -1;
            isDragging = false;
        }
    }


    private void HandleMouseDrag()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0;

        int x, y;
        grid.GetXY(mouseWorldPosition, out x, out y);
        Vector2Int currentCell = new Vector2Int(x, y);
        Vector2Int startingCell = new Vector2Int(startX, startY);
        currentId = cellToPairId.ContainsKey(currentCell) ? cellToPairId[currentCell] : -1;
        bool cellVisited = visitedCells.Any(cell => cell.Item1 == currentCell);
        //int currentId = cellToPairId[currentCell] : cellToPairId[currentCell] != null ? -1;

        if (x >= 0 && y >= 0 && x < gridWidth && y < gridHeight && !currentDragCells.Contains((currentCell, currentId)) && !cellVisited)
        {
            grid.ToggleCellMaterial(x, y, dragMaterial);
            currentDragCells.Add((currentCell, currentId));
            if (currentId != -1)
            {
                Debug.Log($"Dragging over cell: {x}, {y} with ID: {currentId}");
            }
        }
        else if (cellVisited)
        {

            Debug.Log("Touched existing path, invalid");
            ResetTouchedPath(currentCell);
            ResetCurrentDragCells();
        }
        else if (currentId != cellToPairId[startingCell] && currentId!=-1)
        {
            Debug.Log($"Hit another image, invalid, reset");
            HandleMouseUp();
        }
    }


    private void ResetTouchedPath(Vector2Int touchedCell)
    {
        int touchedCellId = -1;
        foreach ((Vector2Int pos, int id) in visitedCells)
        {
            if (pos == touchedCell)
            {
                touchedCellId = id;
                break;
            }
        }

        foreach ((Vector2Int pos, int id) in visitedCells)
        {
            if (id == touchedCellId)
            {
                grid.ResetCell(pos.x, pos.y);
            }
        }
        visitedCells.RemoveWhere(cell => cell.Item2 == touchedCellId);
    }
    private void HandleMouseUp()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0;

        grid.GetXY(mouseWorldPosition, out endX, out endY);

        if (IsValidEndingCell(startX, startY, endX, endY))
        {
            Debug.Log("Successful match!");
            int startId = -1;
            foreach((Vector2Int pos, int id) in currentDragCells)
            {
                if (id != -1)
                {
                    visitedCells.Add((pos, id));
                    startId = id;
                }
                else
                {
                    visitedCells.Add((pos, startId));
                }
            }
        }
        else
        {
            ResetCurrentDragCells();
        }

        isDragging = false;
        startX = startY = endX = endY = -1;
        currentDragCells.Clear();
    }

    private void SetupLevel()
    {
        bool success = false;
        int retryCount = 0;

        while (!success)
        {
            retryCount++;
            Debug.Log($"🔄 Attempt {retryCount}: Generating non-overlapping paths");

            ClearGrid();
            occupiedCells.Clear();
            cellToPairId.Clear();

            DisjointSet disjointSet = new DisjointSet();
            List<Vector2Int> allCells = new List<Vector2Int>();
            for (int x = 0; x < gridWidth; x++)
                for (int y = 0; y < gridHeight; y++)
                    allCells.Add(new Vector2Int(x, y));

            int maxPairs = (gridWidth * gridHeight) / 4;
            int numPairs = Mathf.Min(pairImages.Length / 2, maxPairs);
            ShuffleList(allCells);

            int pairIndex = 0;
            int totalTries = 0;
            while (pairIndex < numPairs && totalTries < 1000)
            {
                totalTries++;

                ShuffleList(allCells);
                Vector2Int? pairStart = null;
                Vector2Int? pairNext = null;

                // Step 1: Find two adjacent unoccupied neighbours to start path
                foreach (var cell in allCells)
                {
                    if (occupiedCells.ContainsKey(cell)) continue;

                    foreach (Vector2Int dir in new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
                    {
                        Vector2Int neighbor = cell + dir;
                        if (!IsInBounds(neighbor) || occupiedCells.ContainsKey(neighbor))
                            continue;

                        pairStart = cell;
                        pairNext = neighbor;
                        break;
                    }
                    if (pairStart != null) break;
                }

                if (pairStart == null || pairNext == null)
                    break;

                List<Vector2Int> path = GrowPath(pairStart.Value, pairNext.Value, disjointSet);
                if (path.Count < 2) continue;

                Vector2Int start = path[0];
                Vector2Int end = path[path.Count - 1];

                // Place images at both ends
                grid.SetCellTexture(start.x, start.y, pairImages[pairIndex * 2]);
                grid.SetCellTexture(end.x, end.y, pairImages[pairIndex * 2 + 1]);
                cellToPairId[start] = pairIndex;
                cellToPairId[end] = pairIndex;

                foreach (var cell in path)
                    occupiedCells[cell] = true;

                pairIndex++;
            }

            if (pairIndex == numPairs && occupiedCells.Count == gridWidth * gridHeight)
            {
                Debug.Log("✅ Successfully placed all pairs without crossing.");
                Debug.Log("🎉 All pairs are connected. Puzzle is ready!");
                success = true;
            }

            else
            {
                Debug.LogWarning("❌ Failed to place all pairs. Retrying...");
            }
        }
    }

    private List<Vector2Int> GrowPath(Vector2Int start, Vector2Int next, DisjointSet ds)
    {
        List<Vector2Int> path = new List<Vector2Int> { start, next };
        ds.MakeSet(start);
        ds.MakeSet(next);
        ds.Union(start, next);

        Vector2Int current = next;
        int attempts = 0;

        while (attempts < 100)
        {
            attempts++;
            List<Vector2Int> neighbors = new List<Vector2Int>();

            foreach (Vector2Int dir in new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
            {
                Vector2Int candidate = current + dir;
                if (!IsInBounds(candidate)) continue;
                if (occupiedCells.ContainsKey(candidate)) continue;
                if (path.Contains(candidate)) continue;

                // Only 1 neighbor in path (current) allowed
                int pathNeighbors = 0;
                foreach (Vector2Int checkDir in new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
                {
                    Vector2Int check = candidate + checkDir;
                    if (path.Contains(check)) pathNeighbors++;
                }

                if (pathNeighbors == 1)
                    neighbors.Add(candidate);
            }

            if (neighbors.Count == 0) break;

            Vector2Int chosen = neighbors[Random.Range(0, neighbors.Count)];
            path.Add(chosen);
            ds.MakeSet(chosen);
            ds.Union(current, chosen);
            current = chosen;
        }

        return path;
    }
    private bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.x < gridWidth && pos.y < gridHeight;
    }



    private List<Vector2Int> FindValidPath(Vector2Int start)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        List<Vector2Int> validEnds = new List<Vector2Int>();

        queue.Enqueue(start);
        visited.Add(start);

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            foreach (Vector2Int dir in new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
            {
                Vector2Int neighbor = current + dir;

                // Out of bounds?
                if (neighbor.x < 0 || neighbor.y < 0 || neighbor.x >= gridWidth || neighbor.y >= gridHeight)
                    continue;

                // Already visited?
                if (visited.Contains(neighbor))
                    continue;

                // Blocked by existing path?
                if (occupiedCells.ContainsKey(neighbor) && neighbor != start)
                    continue;

                // Safe to explore
                visited.Add(neighbor);
                cameFrom[neighbor] = current;
                queue.Enqueue(neighbor);

                // Must be distant enough
                if (ManhattanDistance(start, neighbor) >= 2)
                    validEnds.Add(neighbor);
            }
        }

        if (validEnds.Count > 0)
        {
            Vector2Int chosenEnd = validEnds[Random.Range(0, validEnds.Count)];

            // Reconstruct path
            List<Vector2Int> path = new List<Vector2Int>();
            Vector2Int temp = chosenEnd;

            while (temp != start)
            {
                path.Add(temp);
                temp = cameFrom[temp];
            }
            path.Add(start);
            path.Reverse();
            return path;
        }

        return null;
    }


    private int ManhattanDistance(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private void ShuffleList(List<Vector2Int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Vector2Int temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    private Material DetermineDragMaterial(int x, int y)
    {
        Vector2Int pos = new Vector2Int(x, y);
        if (cellToPairId.ContainsKey(pos))
        {
            int pairId = cellToPairId[pos];
            return dragMaterials[pairId];
        }
        return null;
    }

    private bool IsValidStartingCell(int x, int y)
    {
        return DetermineDragMaterial(x, y) != null;
    }

    private bool IsValidEndingCell(int startX, int startY, int endX, int endY)
    {
        Vector2Int start = new Vector2Int(startX, startY);
        Vector2Int end = new Vector2Int(endX, endY);

        if (!cellToPairId.ContainsKey(start) || !cellToPairId.ContainsKey(end))
            return false;

        return cellToPairId[start] == cellToPairId[end] && start != end;
    }

    private void ResetCurrentDragCells()
    {
        //foreach ((Vector2Int, int) cell in currentDragCells)
        //{
        //    grid.ResetCell(cell.first.x, cell.second.y);
        //}
        //currentDragCells.Clear();
        foreach ((Vector2Int pos, int id) in currentDragCells)
        {
            grid.ResetCell(pos.x, pos.y);
        }
        currentDragCells.Clear();
    }
    private Texture2D[] SubArray(Texture2D[] data, int index, int length)
    {
        Texture2D[] result = new Texture2D[length];
        System.Array.Copy(data, index, result, 0, length);
        return result;
    }
    private void ClearGrid()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                grid.SetCellTexture(x, y, null); // Set to null to remove image
                grid.ResetCell(x, y);            // Also reset any visual drag material
            }
        }

        if (occupiedCells != null) occupiedCells.Clear();
        if (cellToPairId != null) cellToPairId.Clear();
        if (currentDragCells != null) currentDragCells.Clear();
    }

}
