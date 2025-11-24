using UnityEngine;

public class CameraSetup : MonoBehaviour
{
    public Transform gridCenter;
    public Camera cam;

    [Header("Camera Settings")]
    public float height = 50f;
    public float angleX = 50f;
    public float angleY = 45f;
    public float distance = 60f;

    private void Start()
    {
        Vector3 dir = Quaternion.Euler(angleX, angleY, 0) * Vector3.back;
        cam.transform.position = gridCenter.position + dir * distance;

        cam.transform.LookAt(gridCenter.position);
        cam.fieldOfView = 30f;
    }
}
