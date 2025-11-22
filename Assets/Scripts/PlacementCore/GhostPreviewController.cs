using UnityEngine;

public class GhostPreviewController : MonoBehaviour
{
    [SerializeField] private Material ghostMaterial; // static transparent material

    private GameObject currentGhost;

    public void ShowGhost(GameObject prefab)
    {
        HideGhost();

        currentGhost = Instantiate(prefab);
        ApplyGhostMaterial(currentGhost);
    }

    public void UpdateGhostPosition(Vector3 snappedPos)
    {
        if (currentGhost != null)
            currentGhost.transform.position = snappedPos;
    }

    public void HideGhost()
    {
        if (currentGhost != null)
            Destroy(currentGhost);
    }

    private void ApplyGhostMaterial(GameObject obj)
    {
        foreach (var renderer in obj.GetComponentsInChildren<MeshRenderer>())
            renderer.material = ghostMaterial;
    }
}
