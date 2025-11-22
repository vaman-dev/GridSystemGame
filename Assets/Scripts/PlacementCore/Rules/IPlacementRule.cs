public interface IPlacementRule
{
    bool CanPlace(int startX, int startY, Grid grid, BuildingTypeSO building);
}
