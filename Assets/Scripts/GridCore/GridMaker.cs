using UnityEngine;

public class GridMaker : MonoBehaviour
{
    [Header("Grid Settings")]
    public int width = 10;
    public int height = 10;
    [Tooltip("Logical tile size (used for math). We'll compute the world tile size from the plane automatically.")]
    public float logicalTileSize = 1f; // keep for conceptual use; but we compute worldTileSize from plane.

    [Header("Ground Reference")]
    public Transform gridOriginPlane; // assign your plane/ground here

    [HideInInspector]
    public Grid grid;

    private void Awake()
    {
        if (gridOriginPlane == null)
        {
            Debug.LogError("GridMaker: gridOriginPlane not assigned. Please assign the ground/plane transform.");
            return;
        }

        Vector3 planeWorldSize = GetPlaneWorldSize(gridOriginPlane);

        // Compute world tile size from plane width and number of tiles:
        // If the plane's X world-size covers 'width' tiles, each tile in world units is:
        float worldTileSizeX = planeWorldSize.x / width;
        float worldTileSizeZ = planeWorldSize.z / height;

        // (Usually these will be the same if plane is square and tiles are square)
        // We take average to be safe:
        float worldTileSize = (worldTileSizeX + worldTileSizeZ) * 0.5f;

        // bottom-left world position of the grid
        Vector3 origin = gridOriginPlane.position - new Vector3(planeWorldSize.x / 2f, 0f, planeWorldSize.z / 2f);

        // create grid using worldTileSize (world units per tile)
        grid = new Grid(width, height, worldTileSize, origin);

        // Debug info
        Debug.Log($"GridMaker: created grid ({width}x{height}) tileWorldSize={worldTileSize:F3} origin={origin}");
    }

    private Vector3 GetPlaneWorldSize(Transform plane)
    {
        // Unity default Plane mesh is 10x10 world units per localScale 1
        return new Vector3(plane.localScale.x * 10f, 0f, plane.localScale.z * 10f);
    }
}
