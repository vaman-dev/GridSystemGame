using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance;

    [Header("Grid Reference")]
    [SerializeField] private GridMaker gridMaker;

    [Header("Mouse Info")]
    public Vector3 MouseWorldPosition { get; private set; }
    public bool MouseWorldPositionValid { get; private set; }

    public bool LeftClick { get; private set; }
    public bool RightClick { get; private set; }

    public int GridX { get; private set; } = -1;
    public int GridY { get; private set; } = -1;

    [Header("Debug")]
    public bool verboseDebug = false;

    private Camera mainCam;
    private Grid grid;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        mainCam = Camera.main;
    }

    private void Start()
    {
        if (gridMaker == null)
        {
            Debug.LogError("InputHandler: gridMaker not assigned in inspector.");
            enabled = false;
            return;
        }

        grid = gridMaker.grid;
        if (grid == null)
        {
            Debug.LogError("InputHandler: gridMaker.grid is null. Ensure GridMaker created grid in Awake/Start.");
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        UpdateMouseWorldPosition();
        UpdateGridCoordinates();
        CheckClicks();
    }

    private void UpdateMouseWorldPosition()
    {
        // Ray from camera through mouse into scene
        if (mainCam == null)
        {
            mainCam = Camera.main;
            if (mainCam == null)
            {
                MouseWorldPositionValid = false;
                return;
            }
        }

        Ray ray = mainCam.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
        {
            MouseWorldPosition = hit.point;
            MouseWorldPositionValid = true;
        }
        else
        {
            // No hit — invalidate mouse world pos so downstream uses know it is not usable
            MouseWorldPositionValid = false;
            MouseWorldPosition = Vector3.zero;
        }
    }

    private void UpdateGridCoordinates()
    {
        // If ray did not hit anything, mark grid coords invalid
        if (!MouseWorldPositionValid || grid == null)
        {
            GridX = -1;
            GridY = -1;
            return;
        }

        // Use the grid's GetXY (which uses grid origin)
        grid.GetXY(MouseWorldPosition, out int gx, out int gy);
        GridX = gx;
        GridY = gy;

        if (verboseDebug)
        {
            Debug.Log($"InputHandler: MouseWorld {MouseWorldPosition} -> Grid ({GridX},{GridY})");
        }
    }

    private void CheckClicks()
    {
        LeftClick = Mouse.current.leftButton.wasPressedThisFrame;
        RightClick = Mouse.current.rightButton.wasPressedThisFrame;

        if (verboseDebug && LeftClick)
        {
            Debug.Log($"LeftClick → Grid ({GridX},{GridY}) Valid:{MouseWorldPositionValid}");
        }

        if (verboseDebug && RightClick)
        {
            Debug.Log($"RightClick → Grid ({GridX},{GridY}) Valid:{MouseWorldPositionValid}");
        }
    }
}
