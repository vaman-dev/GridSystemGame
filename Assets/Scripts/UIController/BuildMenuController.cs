using UnityEngine;

public class BuildMenuController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Assign the PlacementManager instance (scene)")]
    public PlacementManager placementManager;

    private void Awake()
    {
        if (placementManager == null)
            Debug.LogWarning("BuildMenuController: PlacementManager not assigned in inspector.");
    }

    /// <summary>
    /// Called by UI Button. Pass the BuildingTypeSO asset in the Button OnClick inspector.
    /// Example: Button -> BuildMenuController.OnSelectBuilding -> (drag BuildingTypeSO)
    /// </summary>
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

        placementManager.SelectBuilding(buildingSO);
        Debug.Log($"BuildMenuController: Selected building '{buildingSO.buildingName}'.");
    }

    /// <summary>
    /// Called by Cancel Button or by other UI (e.g., right-click mapped to UI).
    /// </summary>
    public void OnCancelPlacement()
    {
        if (placementManager == null)
        {
            Debug.LogWarning("BuildMenuController: PlacementManager is null. Nothing to cancel.");
            return;
        }

        placementManager.DeselectBuilding();
        Debug.Log("BuildMenuController: Cancelled building placement.");
    }
}
