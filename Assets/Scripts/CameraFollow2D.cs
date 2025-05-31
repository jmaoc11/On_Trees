using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private float moveSpeed = 5f;
    
    [Header("Follow Settings")]
    [SerializeField] private float boundaryThreshold = 1f;
    [SerializeField] private bool followVertically = false;
    [SerializeField] private float leftLimit = 0f;
    [SerializeField] private float rightLimit = 10f;
    [SerializeField] private Vector2 maxPosition = new Vector2(10f, 5f);

    private Camera cam;
    private float initialY;
    private float minX;
    private float maxX;

    private void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("No target assigned to camera!");
            enabled = false;
            return;
        }

        cam = GetComponent<Camera>();
        if (cam == null)
        {
            cam = Camera.main;
        }

        initialY = transform.position.y;
        
        // Calculate camera bounds based on orthographic size and aspect ratio
        float cameraHalfWidth = cam.orthographicSize * cam.aspect;
        minX = leftLimit + cameraHalfWidth;
        maxX = rightLimit - cameraHalfWidth;
        
        // Set initial position to respect boundaries
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        transform.position = pos;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 currentPosition = transform.position;
        Vector3 viewportPoint = cam.WorldToViewportPoint(target.position);

        // Calculate the viewport boundaries with threshold
        float vertExtent = cam.orthographicSize;
        float horizExtent = vertExtent * Screen.width / Screen.height;
        float leftBoundary = 0.5f - (boundaryThreshold / (2f * horizExtent));
        float rightBoundary = 0.5f + (boundaryThreshold / (2f * horizExtent));

        Vector3 targetPosition = currentPosition;

        // Move camera if target is outside boundaries
        if ((viewportPoint.x < leftBoundary && currentPosition.x > minX) || 
            (viewportPoint.x > rightBoundary && currentPosition.x < maxX))
        {
            targetPosition.x = target.position.x;
        }

        // Keep Y position fixed unless followVertically is true
        targetPosition.y = followVertically ? target.position.y : initialY;
        targetPosition.z = currentPosition.z;

        // Clamp to boundaries
        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        if (followVertically)
        {
            targetPosition.y = Mathf.Clamp(targetPosition.y, -maxPosition.y, maxPosition.y);
        }

        // Linear movement towards target position
        transform.position = Vector3.MoveTowards(
            currentPosition,
            targetPosition,
            moveSpeed * Time.deltaTime
        );
    }

    private void OnDrawGizmosSelected()
    {
        if (!cam) cam = GetComponent<Camera>();
        if (!cam) return;

        float cameraHalfWidth = cam.orthographicSize * cam.aspect;
        float minX = leftLimit + cameraHalfWidth;
        float maxX = rightLimit - cameraHalfWidth;

        // Draw camera boundaries in editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(
            new Vector3((minX + maxX) * 0.5f, 0f, 0f),
            new Vector3(maxX - minX, maxPosition.y * 2f, 0.1f)
        );

        // Draw left limit line in red
        Gizmos.color = Color.red;
        Gizmos.DrawLine(
            new Vector3(leftLimit, -maxPosition.y, 0),
            new Vector3(leftLimit, maxPosition.y, 0)
        );

        // Draw right limit line in red
        Gizmos.DrawLine(
            new Vector3(rightLimit, -maxPosition.y, 0),
            new Vector3(rightLimit, maxPosition.y, 0)
        );
    }
}
