using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Movement")]
    public float normalSpeed = 20f;
    public float sprintSpeed = 50f;
    public float movementSmoothTime = 0.15f; // Lower = snappier, Higher = more "slide"

    [Header("Rotation")]
    public float rotateSpeed = 100f;
    public float rotationSmoothTime = 0.1f;

    [Header("Zoom")]
    public float scrollSpeed = 500f;
    public float zoomSmoothTime = 0.2f;
    public Vector2 heightLimits = new Vector2(5f, 50f);

    // Internal velocity variables for SmoothDamp
    private Vector3 currentVelocity;     // Current move speed
    private Vector3 targetVelocity;      // Where we want to go
    private Vector3 velocityRef;         // Helper for SmoothDamp

    private float currentRotation;       // Current Y angle
    private float targetRotation;        // Target Y angle
    private float rotationRef;           // Helper for SmoothDamp

    private float currentHeight;
    private float targetHeight;
    private float heightRef;

    void Start()
    {
        // Initialize targets to current position so we don't snap at start
        targetRotation = transform.eulerAngles.y;
        currentRotation = targetRotation;

        targetHeight = transform.position.y;
        currentHeight = targetHeight;
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleZoom();      
    }

    void HandleMovement()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        // 1. Calculate direction relative to Camera looking direction
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        // Flatten them so we don't fly up/down
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        // Combine inputs with directions
        Vector3 desireDir = (forward * z) + (right * x);

        // 2. Sprint Handling
        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : normalSpeed;

        // 3. Smoothing (Inertia)
        Vector3 targetPos = desireDir * speed;
        // Smoothly change our current velocity towards the target velocity
        currentVelocity = Vector3.SmoothDamp(currentVelocity, targetPos, ref velocityRef, movementSmoothTime);

        // Apply
        transform.position += currentVelocity * Time.deltaTime;
    }

    void HandleRotation()
    {
        if (Input.GetKey(KeyCode.Q)) targetRotation -= rotateSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.E)) targetRotation += rotateSpeed * Time.deltaTime;

        // Smooth rotation
        currentRotation = Mathf.SmoothDampAngle(currentRotation, targetRotation, ref rotationRef, rotationSmoothTime);

        // Apply rotation (Keep X and Z tilt the same, only change Y)
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, currentRotation, 0f);
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        targetHeight -= scroll * scrollSpeed * Time.deltaTime;
        targetHeight = Mathf.Clamp(targetHeight, heightLimits.x, heightLimits.y);

        // Smooth height
        currentHeight = Mathf.SmoothDamp(currentHeight, targetHeight, ref heightRef, zoomSmoothTime);

        Vector3 pos = transform.position;
        pos.y = currentHeight;
        transform.position = pos;
    }
}
