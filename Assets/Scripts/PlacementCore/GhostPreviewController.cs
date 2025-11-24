using UnityEngine;

public class GhostPreviewController : MonoBehaviour
{
    [Header("Materials")]
    [SerializeField] private Material ghostMaterial;       // default translucent material
    [SerializeField] private Material validMaterial;       // optional green material
    [SerializeField] private Material invalidMaterial;     // optional red material

    private GameObject currentGhost;

    public void ShowGhost(GameObject prefab)
    {
        HideGhost();

        if (prefab == null)
        {
            Debug.LogWarning("GhostPreviewController.ShowGhost: prefab is null");
            return;
        }

        currentGhost = Instantiate(prefab);
        ApplyMaterialToGhost(ghostMaterial);
    }

    public void UpdateGhostPosition(Vector3 snappedPos)
    {
        if (currentGhost != null)
            currentGhost.transform.position = snappedPos;
    }

    public void HideGhost()
    {
        if (currentGhost != null)
        {
            Destroy(currentGhost);
            currentGhost = null;
        }
    }

    /// <summary>
    /// Set ghost appearance to valid (true) or invalid (false).
    /// If valid/invalid materials are not assigned, falls back to ghostMaterial.
    /// </summary>
    public void SetValid(bool valid)
    {
        if (currentGhost == null) return;

        Material target = valid ? validMaterial : invalidMaterial;
        if (target == null) target = ghostMaterial;

        ApplyMaterialToGhost(target);
    }

    private void ApplyMaterialToGhost(Material mat)
    {
        if (currentGhost == null || mat == null) return;

        // Apply to MeshRenderer
        var meshRenderers = currentGhost.GetComponentsInChildren<MeshRenderer>(true);
        foreach (var mr in meshRenderers)
        {
            // assign an instance so runtime changes don't affect original material asset
            mr.material = new Material(mat);
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            mr.receiveShadows = false;
        }

        // Apply to SkinnedMeshRenderer
        var skinned = currentGhost.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        foreach (var sk in skinned)
        {
            sk.material = new Material(mat);
            sk.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            sk.receiveShadows = false;
        }
    }

    public void SetRotation(Quaternion rot)
    {
        if (currentGhost == null) return;
        currentGhost.transform.rotation = rot;
    }
}
