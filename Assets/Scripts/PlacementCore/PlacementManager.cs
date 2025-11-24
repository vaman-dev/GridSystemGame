using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    [Header("References")]
    public GridMaker gridMaker;                         // required (assign in inspector)
    public GridVisualizer gridVisualizer;               // required (assign in inspector)
    public PlacementValidator placementValidator;       // required (assign in inspector)
    public GhostPreviewController ghostPreview;         // optional but recommended

    [Header("Default")]
    public BuildingTypeSO currentBuildingSO;            // assigned by UI when player selects building
    public GameObject defaultBuildingPrefab;            // fallback prefab

    [Header("Debug")]
    public bool verboseDebug = false;

    private Grid grid;
    public BuildingTypeSO CurrentBuilding => currentBuildingSO;

    private void Start()
    {
        if (gridMaker == null)
        {
            Debug.LogError("PlacementManager: GridMaker reference missing.");
            enabled = false;
            return;
        }

        grid = gridMaker.grid;
        if (grid == null)
        {
            Debug.LogError("PlacementManager: GridMaker.grid is null.");
            enabled = false;
            return;
        }

        if (placementValidator == null)
        {
            Debug.LogError("PlacementManager: PlacementValidator reference missing.");
            enabled = false;
            return;
        }

        if (gridVisualizer == null)
        {
            Debug.LogError("PlacementManager: GridVisualizer reference missing.");
            enabled = false;
            return;
        }

        if (ghostPreview == null)
        {
            Debug.LogWarning("PlacementManager: GhostPreviewController not assigned. Preview will not show.");
        }
    }

    private void Update()
    {
        // Only run placement preview when a building is selected
        if (currentBuildingSO == null)
        {
            gridVisualizer.ClearPlacementTiles();
            ghostPreview?.HideGhost();
            return;
        }

        if (InputHandler.Instance == null)
        {
            if (verboseDebug) Debug.LogWarning("PlacementManager: InputHandler.Instance is null.");
            return;
        }

        int gx = InputHandler.Instance.GridX;
        int gy = InputHandler.Instance.GridY;
        bool mouseValid = InputHandler.Instance.MouseWorldPositionValid;

        // If mouse not over valid ground -> clear visuals and hide ghost
        if (!mouseValid || gx < 0 || gy < 0 || !grid.IsInsideGrid(gx, gy))
        {
            gridVisualizer.ClearPlacementTiles();
            ghostPreview?.HideGhost();
            if (verboseDebug) Debug.Log($"PlacementManager: mouse not over grid or outside bounds (Grid: {gx},{gy}, Valid:{mouseValid}).");
            return;
        }

        // Snap to the starting tile center (bottom-left anchor)
        Vector3 snappedWorld = grid.GetWorldPosition(gx, gy);
        snappedWorld.y = 0.01f;

        // Update ghost preview
        ghostPreview?.UpdateGhostPosition(snappedWorld);

        // Validate placement (only when mouse is over valid tile inside grid)
        bool isValid = placementValidator.CanPlace(gx, gy, currentBuildingSO, grid);

        // Visualize tiles under footprint using gridVisualizer
        gridVisualizer.ShowPlacementTiles(gx, gy, currentBuildingSO.tileWidth, currentBuildingSO.tileHeight, isValid);

        if (InputHandler.Instance.LeftClick)
        {
            if (isValid)
                PlaceBuildingAt(gx, gy, snappedWorld);
            else
                Debug.Log($"PlacementManager: Placement blocked at ({gx},{gy}) for '{currentBuildingSO.buildingName}'.");
        }
    }

    private void PlaceBuildingAt(int startX, int startY, Vector3 worldPos)
    {
        GameObject prefabToPlace = currentBuildingSO != null && currentBuildingSO.prefab != null
            ? currentBuildingSO.prefab
            : defaultBuildingPrefab;

        if (prefabToPlace == null)
        {
            Debug.LogError("PlacementManager: No prefab assigned to place.");
            return;
        }

        GameObject newObj = Instantiate(prefabToPlace, worldPos, Quaternion.identity);
        newObj.name = $"{prefabToPlace.name}_({startX},{startY})";

        placementValidator.SetOccupied(startX, startY, currentBuildingSO, grid);

        gridVisualizer.ClearPlacementTiles();
        ghostPreview?.HideGhost();

        Debug.Log($"PlacementManager: Placed '{newObj.name}' at grid ({startX},{startY}).");
    }

    // Called by UI
    public void SelectBuilding(BuildingTypeSO building)
    {
        currentBuildingSO = building;

        if (building != null)
        {
            ghostPreview?.ShowGhost(building.prefab);
            if (verboseDebug)
                Debug.Log($"PlacementManager: Selected building '{building.buildingName}' size {building.tileWidth}x{building.tileHeight}.");
        }
        else
        {
            ghostPreview?.HideGhost();
            gridVisualizer.ClearPlacementTiles();

            if (verboseDebug) Debug.Log("PlacementManager: Deselected building.");
        }
    }

    public void DeselectBuilding()
    {
        currentBuildingSO = null;
        ghostPreview?.HideGhost();
        gridVisualizer.ClearPlacementTiles();
    }

}
