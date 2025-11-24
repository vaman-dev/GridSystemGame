using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GridVisualizer : MonoBehaviour
{
    public GridMaker gridSystem;          // assign GridMaker in inspector

    [Header("Placement Materials")]
    [SerializeField] private Material validMaterial;
    [SerializeField] private Material invalidMaterial;


    [SerializeField] private float quadYOffset = 0.02f;

    public Vector3 GridOriginPosition => gridSystem.grid.Origin;
    private readonly List<GameObject> activeQuads = new List<GameObject>();

    private void OnDrawGizmos()
    {
        if (gridSystem == null || gridSystem.grid == null) return;

        DrawGridLines();
        DrawGridNumbers();
    }

    private void DrawGridLines()
    {
        Grid grid = gridSystem.grid;
        Vector3 origin = grid.Origin;

        for (int x = 0; x <= grid.Width; x++)
        {
            Vector3 start = origin + new Vector3(x * grid.TileSize, 0, 0);
            Vector3 end = origin + new Vector3(x * grid.TileSize, 0, grid.Height * grid.TileSize);
            Debug.DrawLine(start, end, Color.white);
        }

        for (int y = 0; y <= grid.Height; y++)
        {
            Vector3 start = origin + new Vector3(0, 0, y * grid.TileSize);
            Vector3 end = origin + new Vector3(grid.Width * grid.TileSize, 0, y * grid.TileSize);
            Debug.DrawLine(start, end, Color.white);
        }
    }

    private void DrawGridNumbers()
    {
#if UNITY_EDITOR
        Grid grid = gridSystem.grid;

        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                Vector3 tileCenter = grid.GetWorldPosition(x, y) + Vector3.up * 0.1f;
                Handles.Label(tileCenter, $"({x},{y})");
            }
        }
#endif
    }

    // Place quads using grid.GetWorldPosition so they always align

    public void ShowPlacementTiles(int startX, int startY, int width, int height, bool isValid)
    {
        if (gridSystem == null || gridSystem.grid == null) return;

        ClearPlacementTiles();

        Grid grid = gridSystem.grid;
        Material selectedMat = isValid ? validMaterial : invalidMaterial;

        // world actual tile size (computed in GridMaker)
        float tileWorld = grid.TileSize;
        float thickness = (QuadObjectPooler.Instance != null) ? QuadObjectPooler.Instance.quadThickness : 0.01f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int gx = startX + x;
                int gy = startY + y;

                if (!grid.IsInsideGrid(gx, gy)) continue;

                // --- Place quad so its bottom sits at ground ---
                // If quad pivot is centered, set center = groundY + thickness/2
                Vector3 worldPos = grid.GetWorldPosition(gx, gy) + new Vector3(0, quadYOffset, 0);


                GameObject quad = QuadObjectPooler.Instance.GetQuad();
                if (quad == null) continue;

                quad.transform.SetParent(this.transform, true);
                quad.transform.position = worldPos;

                // flatten to ground (XZ plane)
                quad.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

                // square scaling based on real tile size
                quad.transform.localScale = new Vector3(tileWorld, thickness, tileWorld);

                // apply material
                var mr = quad.GetComponent<MeshRenderer>();
                if (mr != null && selectedMat != null) mr.material = selectedMat;

                activeQuads.Add(quad);
            }
        }
    }



    public void ClearPlacementTiles()
    {
        for (int i = activeQuads.Count - 1; i >= 0; i--)
        {
            var q = activeQuads[i];
            if (q != null) QuadObjectPooler.Instance.ReturnQuad(q);
        }
        activeQuads.Clear();
    }
}
