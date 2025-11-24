using System.Collections.Generic;
using UnityEngine;

public class QuadObjectPooler : MonoBehaviour
{
    public static QuadObjectPooler Instance;

    [Header("Pool Settings")]
    public GameObject quadPrefab;
    public int poolSize = 200;

    [Header("Quad appearance")]
    [Tooltip("Thin thickness so quad sits above the ground. Mesh pivot assumed centered.")]
    public float quadThickness = 0.01f;

    private Queue<GameObject> quadPool;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        quadPool = new Queue<GameObject>();
        InitializePool();
    }

    private void InitializePool()
    {
        if (quadPrefab == null)
        {
            Debug.LogError("QuadObjectPooler: quadPrefab not assigned.");
            return;
        }

        for (int i = 0; i < poolSize; i++)
        {
            GameObject quad = Instantiate(quadPrefab, transform);
            // keep prefab's transform neutral so visualizer sets rotation/scale
            quad.transform.localPosition = Vector3.zero;
            quad.transform.localRotation = Quaternion.identity;
            quad.transform.localScale = Vector3.one;
            quad.SetActive(false);
            quadPool.Enqueue(quad);
        }
    }

    public GameObject GetQuad()
    {
        if (quadPool.Count == 0) ExpandPool();
        var q = quadPool.Dequeue();
        q.SetActive(true);
        return q;
    }

    public void ReturnQuad(GameObject quad)
    {
        if (quad == null) return;
        quad.SetActive(false);
        quad.transform.SetParent(this.transform, true);
        quadPool.Enqueue(quad);
    }

    private void ExpandPool()
    {
        GameObject quad = Instantiate(quadPrefab, transform);
        quad.transform.localPosition = Vector3.zero;
        quad.transform.localRotation = Quaternion.identity;
        quad.transform.localScale = Vector3.one;
        quad.SetActive(false);
        quadPool.Enqueue(quad);
    }
}
