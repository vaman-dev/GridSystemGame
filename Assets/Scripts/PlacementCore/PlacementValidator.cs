using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PlacementValidator: validates whether a BuildingTypeSO can be placed at a given start tile.
/// Supports rotated footprints using CanPlaceBuildingGrid(width,height).
/// </summary>
public class PlacementValidator : MonoBehaviour
{
    /// <summary>
    /// Standard entry function for fixed-width buildings (non-rotated).
    /// PlacementManager now calls CanPlaceBuildingGrid() instead when rotation active.
    /// </summary>
    public bool CanPlace(int startX, int startY, BuildingTypeSO building, Grid grid)
    {
        if (building == null || grid == null)
        {
            Debug.LogWarning("Placement failed: BuildingTypeSO or Grid reference missing.");
            return false;
        }

        int width = Mathf.Max(1, building.tileWidth);
        int height = Mathf.Max(1, building.tileHeight);

        bool result;
        switch (building.placementRuleType)
        {
            case PlacementRuleType.SingleTile:
                result = CanPlaceSingle(startX, startY, grid);
                break;

            case PlacementRuleType.MultiTile:
            case PlacementRuleType.RoadTile:
            case PlacementRuleType.DecorationTile:
                result = CanPlaceMulti(startX, startY, width, height, grid);
                break;

            default:
                result = CanPlaceMulti(startX, startY, width, height, grid);
                break;
        }

        if (!result)
        {
            Vector2Int? blocked = GetFirstBlockedTile(startX, startY, building, grid);
            if (blocked.HasValue)
            {
                var b = blocked.Value;
                if (!grid.IsInsideGrid(b.x, b.y))
                    Debug.Log($"❌ Out of bounds at ({b.x},{b.y})");
                else
                    Debug.Log($"❌ Occupied at ({b.x},{b.y})");
            }
        }

        return result;
    }

    // -------------------------------------------------------------------------
    // ROTATION-AWARE VALIDATION
    // -------------------------------------------------------------------------
    /// <summary>
    /// Validate placement with explicit width & height (used after rotation).
    /// </summary>
    public bool CanPlaceBuildingGrid(int gridX, int gridY, BuildingTypeSO buildingData, int checkWidth, int checkHeight, Grid grid)
    {
        if (buildingData == null || grid == null)
        {
            Debug.LogError("PlacementValidator: buildingData or grid is null");
            return false;
        }

        for (int x = 0; x < checkWidth; x++)
        {
            for (int y = 0; y < checkHeight; y++)
            {
                int tx = gridX + x;
                int ty = gridY + y;

                if (!grid.IsInsideGrid(tx, ty) || grid.IsOccupied(tx, ty))
                    return false;
            }
        }

        return true;
    }


    // -------------------------------------------------------------------------
    // Base multi / single logic
    // -------------------------------------------------------------------------
    private bool CanPlaceSingle(int x, int y, Grid grid)
    {
        if (!grid.IsInsideGrid(x, y)) return false;
        return !grid.IsOccupied(x, y);
    }

    private bool CanPlaceMulti(int startX, int startY, int width, int height, Grid grid)
    {
        for (int dx = 0; dx < width; dx++)
        {
            for (int dy = 0; dy < height; dy++)
            {
                int gx = startX + dx;
                int gy = startY + dy;

                if (!grid.IsInsideGrid(gx, gy)) return false;
                if (grid.IsOccupied(gx, gy)) return false;
            }
        }
        return true;
    }

    // -------------------------------------------------------------------------
    // Utility functions
    // -------------------------------------------------------------------------
    public List<Vector2Int> GetCoveredTiles(int startX, int startY, BuildingTypeSO building, Grid grid)
    {
        var tiles = new List<Vector2Int>();
        if (building == null || grid == null) return tiles;

        int width = Mathf.Max(1, building.tileWidth);
        int height = Mathf.Max(1, building.tileHeight);

        for (int dx = 0; dx < width; dx++)
        {
            for (int dy = 0; dy < height; dy++)
            {
                int gx = startX + dx;
                int gy = startY + dy;
                if (grid.IsInsideGrid(gx, gy))
                    tiles.Add(new Vector2Int(gx, gy));
            }
        }
        return tiles;
    }

    public void SetOccupied(int startX, int startY, BuildingTypeSO building, Grid grid)
    {
        if (building == null || grid == null) return;

        int width = Mathf.Max(1, building.tileWidth);
        int height = Mathf.Max(1, building.tileHeight);

        for (int dx = 0; dx < width; dx++)
        {
            for (int dy = 0; dy < height; dy++)
            {
                grid.SetOccupied(startX + dx, startY + dy, true);
            }
        }
    }

    public void ClearOccupied(int startX, int startY, BuildingTypeSO building, Grid grid)
    {
        if (building == null || grid == null) return;

        int width = Mathf.Max(1, building.tileWidth);
        int height = Mathf.Max(1, building.tileHeight);

        for (int dx = 0; dx < width; dx++)
        {
            for (int dy = 0; dy < height; dy++)
            {
                grid.SetOccupied(startX + dx, startY + dy, false);
            }
        }
    }

    public Vector2Int? GetFirstBlockedTile(int startX, int startY, BuildingTypeSO building, Grid grid)
    {
        if (building == null || grid == null) return null;

        int width = Mathf.Max(1, building.tileWidth);
        int height = Mathf.Max(1, building.tileHeight);

        for (int dx = 0; dx < width; dx++)
        {
            for (int dy = 0; dy < height; dy++)
            {
                int gx = startX + dx;
                int gy = startY + dy;

                if (!grid.IsInsideGrid(gx, gy)) return new Vector2Int(gx, gy);
                if (grid.IsOccupied(gx, gy)) return new Vector2Int(gx, gy);
            }
        }

        return null;
    }
}
