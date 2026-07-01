using UnityEngine;

public class CameraFollowWithBounds : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Bounds")]
    [SerializeField] private BoxCollider2D cameraBounds;

    [Header("Settings")]
    [SerializeField] private float smoothSpeed = 8f;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (player == null || cameraBounds == null || cam == null) return;

        // Camera size in world units
        float cameraHalfHeight = cam.orthographicSize;
        float cameraHalfWidth = cameraHalfHeight * cam.aspect;

        // Boundary edges
        Bounds bounds = cameraBounds.bounds;

        float minX = bounds.min.x + cameraHalfWidth;
        float maxX = bounds.max.x - cameraHalfWidth;
        float minY = bounds.min.y + cameraHalfHeight;
        float maxY = bounds.max.y - cameraHalfHeight;

        // Start by following the player
        float targetX = player.position.x;
        float targetY = player.position.y;

        // If bounds are wide enough, clamp X
        // If not, lock X to boundary centre
        if (minX <= maxX)
        {
            targetX = Mathf.Clamp(targetX, minX, maxX);
        }
        else
        {
            targetX = bounds.center.x;
        }

        // If bounds are tall enough, clamp Y
        // If not, lock Y to boundary centre
        if (minY <= maxY)
        {
            targetY = Mathf.Clamp(targetY, minY, maxY);
        }
        else
        {
            targetY = bounds.center.y;
        }

        Vector3 targetPosition = new Vector3(targetX, targetY, -10f);

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );
    }
}