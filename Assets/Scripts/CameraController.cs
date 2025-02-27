using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private CameraControls controls;
    private Vector2 moveInput;
    private float zoomInput;

    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float zoomSpeed = 25f;
    [SerializeField] private float resetZoom = 5f;

    private Vector3 resetPosition;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        controls = new CameraControls();

        controls.Camera.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Camera.Move.canceled += _ => moveInput = Vector2.zero;

        controls.Camera.Zoom.performed += ctx => zoomInput = ctx.ReadValue<float>();
        controls.Camera.Zoom.canceled += _ => zoomInput = 0f;

        controls.Camera.ResetCamera.performed += _ => ResetCamera();

        controls.Enable();

        SetResetPosition(GridManager.Instance.gridWidth, GridManager.Instance.gridHeight);
    }

    private void OnDisable()
    {
        if (controls != null)
        {
            controls.Disable();
        }
    }

    public void SetResetPosition(float x, float y)
    {
        resetPosition = new Vector3(x, y, -10);
    }

    private void Start()
    {
        ResetCamera();
    }

    private void Update()
    {
        if (Application.isFocused && IsMouseWithinGameWindow())
        {
            Vector3 move = new Vector3(moveInput.x, moveInput.y, 0) * (moveSpeed * Time.deltaTime);
            transform.position += move;

            if (zoomInput != 0)
            {
                cam.orthographicSize -= zoomInput * zoomSpeed * Time.deltaTime;
                cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, 2f, 20f);
            }
        }
    }

    public void ResetCamera()
    {
        transform.position = resetPosition;
        cam.orthographicSize = resetZoom;
    }

    private bool IsMouseWithinGameWindow()
    {
        Vector3 mousePosition = Mouse.current.position.ReadValue();
        return mousePosition.x >= 0 && mousePosition.x <= Screen.width && mousePosition.y >= 0 && mousePosition.y <= Screen.height;
    }
}