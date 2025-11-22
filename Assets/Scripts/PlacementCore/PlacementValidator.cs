using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PlacementValidator: validates whether a BuildingTypeSO can be placed at a given start tile.
/// - Works purely in grid-space using Grid (pure C# class).
/// - Uses BuildingTypeSO.placementRuleType to decide rule behavior.
/// - Provides helpers: CanPlace, GetCoveredTiles, SetOccupied, ClearOccupied.
/// - Now logs helpful debug messages when placement fails (occupied tile or out-of-bounds).
/// </summary>
public class PlacementValidator : MonoBehaviour
{
    /// <summary>
    /// High level API. Call from PlacementManager before placing.
    /// </summary>
    /// <param name="startX">Start grid X</param>
    /// <param name="startY">Start grid Y</param>
    /// <param name="building">BuildingTypeSO selected</param>
    /// <param name="grid">Grid instance</param>
    /// <returns>True if the footprint can be placed</returns>
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
                result = CanPlaceMulti(startX, startY, width, height, grid);
                break;

            case PlacementRuleType.RoadTile:
                // For now treat roads same as multi-tile occupancy.
                // Future: add adjacency/connection checks here.
                result = CanPlaceMulti(startX, startY, width, height, grid);
                break;

            case PlacementRuleType.DecorationTile:
                // Decorations may have special rules later; for now same as multi.
                result = CanPlaceMulti(startX, startY, width, height, grid);
                break;

            default:
                result = CanPlaceMulti(startX, startY, width, height, grid);
                break;
        }

        if (!result)
        {
            // Try to get the first blocked tile and give a useful debug message
            Vector2Int? blocked = GetFirstBlockedTile(startX, startY, building, grid);
            if (blocked.HasValue)
            {
                var b = blocked.Value;
                if (!grid.IsInsideGrid(b.x, b.y))
                {
                    Debug.Log($"❌ Cannot place '{building.buildingName}' at ({startX},{startY}): tile ({b.x},{b.y}) is outside grid bounds.");
                }
                else
                {
                    Debug.Log($"❌ Cannot place '{building.buildingName}' at ({startX},{startY}): tile ({b.x},{b.y}) is already occupied.");
                }
            }
            else
            {
                // Fallback generic message
                Debug.Log($"❌ Cannot place '{building.buildingName}' at ({startX},{startY}). PlacementValidator returned false.");
            }
        }

        return result;
    }

    // 1x1 placement check
    private bool CanPlaceSingle(int x, int y, Grid grid)
    {
        if (!grid.IsInsideGrid(x, y)) return false;
        return !grid.IsOccupied(x, y);
    }

    // Multi-tile check (width x height)
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

    /// <summary>
    /// Returns all tiles that the building would cover (only tiles inside grid).
    /// Useful for visualizing footprint (GridVisualizer) or marking occupancy.
    /// </summary>
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

    /// <summary>
    /// Mark footprint tiles as occupied in the Grid instance. Call after a successful placement.
    /// </summary>
    public void SetOccupied(int startX, int startY, BuildingTypeSO building, Grid grid)
    {
        if (building == null || grid == null) return;

        int width = Mathf.Max(1, building.tileWidth);
        int height = Mathf.Max(1, building.tileHeight);

        for (int dx = 0; dx < width; dx++)
        {
            for (int dy = 0; dy < height; dy++)
            {
                int gx = startX + dx;
                int gy = startY + dy;
                grid.SetOccupied(gx, gy, true);
            }
        }
    }

    /// <summary>
    /// Clear occupancy for a footprint (used when removing/dismantling).
    /// </summary>
    public void ClearOccupied(int startX, int startY, BuildingTypeSO building, Grid grid)
    {
        if (building == null || grid == null) return;

        int width = Mathf.Max(1, building.tileWidth);
        int height = Mathf.Max(1, building.tileHeight);

        for (int dx = 0; dx < width; dx++)
        {
            for (int dy = 0; dy < height; dy++)
            {
                int gx = startX + dx;
                int gy = startY + dy;
                grid.SetOccupied(gx, gy, false);
            }
        }
    }

    // Optional helper: returns the first tile that blocks placement (or null)
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
