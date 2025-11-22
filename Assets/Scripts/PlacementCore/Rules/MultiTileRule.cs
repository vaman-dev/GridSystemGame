using UnityEngine;

[CreateAssetMenu(fileName = "MultiTileRule", menuName = "GridBuilding/Rules/Multi Tile Rule")]
public class MultiTileRule : ScriptableObject, IPlacementRule
{
    public bool CanPlace(int startX, int startY, Grid grid, BuildingTypeSO building)
    {
        for (int x = 0; x < building.tileWidth; x++)
        {
            for (int y = 0; y < building.tileHeight; y++)
            {
                int gx = startX + x;
                int gy = startY + y;

                if (!grid.IsInsideGrid(gx, gy)) return false;
                if (grid.IsOccupied(gx, gy)) return false;
            }
        }

        return true;
    }
}
