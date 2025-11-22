using UnityEngine;

[CreateAssetMenu(fileName = "SingleTileRule", menuName = "GridBuilding/Rules/Single Tile Rule")]
public class SingleTileRule : ScriptableObject, IPlacementRule
{
    public bool CanPlace(int startX, int startY, Grid grid, BuildingTypeSO building)
    {
        if (!grid.IsInsideGrid(startX, startY)) return false;
        return !grid.IsOccupied(startX, startY);
    }
}
