using UnityEngine;

public class BuildMenuController : MonoBehaviour
{
    [Header("References")]
    public PlacementManager placementManager; 

    private void Awake()
    {
        if (placementManager == null)
            Debug.LogWarning("BuildMenuController: PlacementManager not assigned in inspector.");
    }

    // Called by UI buttons (pass BuildingTypeSO in the inspector)
    public void OnSelectBuilding(BuildingTypeSO buildingSO)
    {
        if (buildingSO == null)
        {
            Debug.LogError("BuildMenuController: Selected BuildingTypeSO is null!");
            return;
        }

        if (placementManager == null)
        {
            Debug.LogError("BuildMenuController: PlacementManager is null. Cannot select building.");
            return;
        }

        // Optional: toggle off if same building selected twice
        if (placementManager.CurrentBuilding == buildingSO)
        {
            placementManager.DeselectBuilding();
            Debug.Log($"UI: Deselected building {buildingSO.buildingName}");
            return;
        }

        placementManager.SelectBuilding(buildingSO);
        Debug.Log($"UI: Selected building {buildingSO.buildingName}");
    }

    // Called by Cancel Button or Right-Click
    public void OnCancelPlacement()
    {
        if (placementManager == null)
        {
            Debug.LogWarning("BuildMenuController: PlacementManager is null. Nothing to cancel.");
            return;
        }

        placementManager.DeselectBuilding();
        Debug.Log("UI: Cancelled building placement.");
    }
}
