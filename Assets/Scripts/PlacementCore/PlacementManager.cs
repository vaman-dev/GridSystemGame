using UnityEngine;

/// <summary>
/// PlacementManager
/// - UI must call SelectBuilding(buildingSO) to start placement.
/// - Right-click or ESC cancels placement.
/// - No fallback/default prefab — placement only when UI has selected a BuildingTypeSO.
/// </summary>
public class PlacementManager : MonoBehaviour
{
    public enum AnchorMode { Center, BottomLeft }

    [Header("References")]
    public GridMaker gridMaker;
    public GridVisualizer gridVisualizer;
    public PlacementValidator placementValidator;
    public GhostPreviewController ghostPreview;

    [Header("Debug")]
    public bool verboseDebug = false;

    [Header("Snapping")]
    public AnchorMode anchorMode = AnchorMode.Center;
    [Tooltip("Y offset for ghost / placement (set in inspector).")]
    public float placementHeightOffset = 0.01f;

    [Header("Rotation")]
    [Tooltip("Current rotation applied to the preview and final placement")]
    public Quaternion currentRotation = Quaternion.identity;

    [Tooltip("If true, rotating 90/270 swaps the building footprint (tileWidth/tileHeight)")]
    public bool rotateFootprint = true;

    private Grid grid;

    // NOTE: private — only set via SelectBuilding called by UI (BuildMenuController)
    private BuildingTypeSO currentBuildingSO;
    public BuildingTypeSO CurrentBuilding => currentBuildingSO; // read-only accessor

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

        // NOTE: We intentionally do NOT auto-overwrite placementHeightOffset here
        // so inspector values remain authoritative.
    }

    private void Update()
    {
        // Cancel / Deselect input: right click or Escape
        if (Input.GetKeyDown(KeyCode.Escape) || (InputHandler.Instance != null && InputHandler.Instance.RightClick) || Input.GetMouseButtonDown(1))
        {
            if (currentBuildingSO != null)
            {
                DeselectBuilding();
                return;
            }
        }

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
            ghostPreview?.SetValid(false);   // keep ghost but show invalid color
            return;
        }

        // --- Calculate Rotated Footprint ---
        int footW, footH;
        GetRotatedFootprint(currentBuildingSO, out footW, out footH);

        // Snap final world position (uses footprint dims for center anchor correctness)
        Vector3 snappedWorld = GetSnappedWorldPosition(gx, gy, currentBuildingSO, footW, footH);
        snappedWorld.y = placementHeightOffset;

        // Ghost position update
        ghostPreview?.UpdateGhostPosition(snappedWorld);

        // Apply rotation continuously so ghost remains rotated
        ghostPreview?.SetRotation(currentRotation);

        // Validate with rotated footprint
        bool isValid = placementValidator.CanPlaceBuildingGrid(gx, gy, currentBuildingSO, footW, footH, grid);

        // Set ghost material based on validation
        ghostPreview?.SetValid(isValid);

        // Visualize rotated footprint grid tiles
        gridVisualizer.ShowPlacementTiles(gx, gy, footW, footH, isValid);

        // Place on left click
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
        if (currentBuildingSO == null || currentBuildingSO.prefab == null)
        {
            Debug.LogError("PlacementManager: Cannot place - Building SO or prefab missing. Make sure UI selected a building.");
            return;
        }

        // Instantiate with stored rotation
        GameObject newObj = Instantiate(currentBuildingSO.prefab, worldPos, currentRotation);
        newObj.name = $"{currentBuildingSO.prefab.name}_({startX},{startY})";

        placementValidator.SetOccupied(startX, startY, currentBuildingSO, grid);

        gridVisualizer.ClearPlacementTiles();

        // Show ghost again for continuous placement
        ghostPreview?.ShowGhost(currentBuildingSO.prefab);
        ghostPreview?.SetRotation(currentRotation);
        
        // If you want single placement, uncomment:
        // DeselectBuilding();

        Debug.Log($"PlacementManager: Placed '{newObj.name}' at grid ({startX},{startY}) rotated {currentRotation.eulerAngles.y}°");
    }

    /// <summary>
    /// Called by UI to start placement (BuildMenuController should call this).
    /// </summary>
    public void SelectBuilding(BuildingTypeSO building)
    {
        currentBuildingSO = building;
        currentRotation = Quaternion.identity; // reset rotation when selecting a new building

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

    /// <summary>
    /// Deselect/cancel placement (can be called by UI or by right-click / ESC).
    /// </summary>
    public void DeselectBuilding()
    {
        currentBuildingSO = null;
        currentRotation = Quaternion.identity;
        ghostPreview?.HideGhost();
        gridVisualizer.ClearPlacementTiles();

        if (verboseDebug) Debug.Log("PlacementManager: Deselected building (cancelled).");
    }

    /// <summary>
    /// Get world position to place ghost / prefab.
    /// Uses grid origin and tile size. Accepts rotated footprint dims so center anchor works correctly.
    /// </summary>
    private Vector3 GetSnappedWorldPosition(int startX, int startY, BuildingTypeSO building, int footprintW, int footprintH)
    {
        Vector3 origin = gridVisualizer.GridOriginPosition;
        float tile = grid.TileSize;

        int w = Mathf.Max(1, footprintW);
        int h = Mathf.Max(1, footprintH);

        if (anchorMode == AnchorMode.Center)
        {
            float centerX = startX * tile + (w * tile) / 2f;
            float centerZ = startY * tile + (h * tile) / 2f;
            return origin + new Vector3(centerX, 0f, centerZ);
        }
        else // BottomLeft
        {
            float cornerX = startX * tile;
            float cornerZ = startY * tile;
            return origin + new Vector3(cornerX, 0f, cornerZ);
        }
    }

    /// <summary>
    /// Rotate preview/final placement by degrees (call from RotationController).
    /// </summary>
    public void RotateByDegrees(float degrees)
    {
        currentRotation *= Quaternion.Euler(0f, degrees, 0f);

        float y = Mathf.Round(currentRotation.eulerAngles.y / 90f) * 90f;
        currentRotation = Quaternion.Euler(0f, y, 0f);

        if (currentBuildingSO != null)
        {
            ghostPreview?.SetRotation(currentRotation);
            // Force visual refresh — DO NOT HideGhost()
            gridVisualizer.ClearPlacementTiles();
        }
    }

    private void GetRotatedFootprint(BuildingTypeSO building, out int w, out int h)
    {
        w = (building != null) ? Mathf.Max(1, building.tileWidth) : 1;
        h = (building != null) ? Mathf.Max(1, building.tileHeight) : 1;

        if (!rotateFootprint || building == null) return;

        int steps = Mathf.RoundToInt(currentRotation.eulerAngles.y / 90f) % 4;
        if (steps < 0) steps += 4;

        if (steps == 1 || steps == 3)
        {
            int temp = w;
            w = h;
            h = temp;
        }
    }
}
