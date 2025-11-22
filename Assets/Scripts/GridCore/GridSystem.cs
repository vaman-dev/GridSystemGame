using UnityEngine;

public class Grid
{
    public int Width { get; private set; }
    public int Height { get; private set; }
    public float TileSize { get; private set; }

    // World-space bottom-left origin (world coordinates)
    public Vector3 Origin { get; private set; }

    private bool[,] occupied;

    // New constructor includes the world origin (bottom-left of the grid)
    public Grid(int width, int height, float tileSize, Vector3 origin)
    {
        Width = width;
        Height = height;
        TileSize = tileSize;
        Origin = origin;

        occupied = new bool[width, height];
    }

    // World → Grid (uses Origin to convert)
    public void GetXY(Vector3 worldPos, out int x, out int y)
    {
        Vector3 local = worldPos - Origin; // now local.x and local.z are positive inside the grid
        x = Mathf.FloorToInt(local.x / TileSize);
        y = Mathf.FloorToInt(local.z / TileSize);
    }

    // Grid → World center position of tile
    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(
            Origin.x + x * TileSize + TileSize / 2f,
            Origin.y,
            Origin.z + y * TileSize + TileSize / 2f
        );
    }

    public bool IsInsideGrid(int x, int y)
    {
        return x >= 0 && y >= 0 && x < Width && y < Height;
    }

    public bool IsOccupied(int x, int y)
    {
        if (!IsInsideGrid(x, y)) return true;
        return occupied[x, y];
    }

    public void SetOccupied(int x, int y, bool value)
    {
        if (IsInsideGrid(x, y))
            occupied[x, y] = value;
    }
}
