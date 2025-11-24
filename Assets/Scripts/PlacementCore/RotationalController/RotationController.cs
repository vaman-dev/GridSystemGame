using UnityEngine;

public class RotationController : MonoBehaviour
{
    [Tooltip("Reference to PlacementManager (assign in inspector)")]
    public PlacementManager placementManager;

    [Tooltip("Degrees to rotate with each press (typically 90)")]
    public float rotationStep = 90f;

    private void Update()
    {
        if (placementManager == null) return;

        // rotate clockwise on R
        if (Input.GetKeyDown(KeyCode.R))
        {
            placementManager.RotateByDegrees(rotationStep);
        }

        // optional: rotate anti-clockwise with Q
        if (Input.GetKeyDown(KeyCode.Q))
        {
            placementManager.RotateByDegrees(-rotationStep);
        }
    }

    // Expose for UI button: call this from Button OnClick() to rotate clockwise
    public void RotateClockwise()
    {
        if (placementManager == null) return;
        placementManager.RotateByDegrees(rotationStep);
    }

    // Expose for UI button: counter-clockwise
    public void RotateCounterClockwise()
    {
        if (placementManager == null) return;
        placementManager.RotateByDegrees(-rotationStep);
    }
}
