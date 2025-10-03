using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Setup")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 2f, -10f);

    [Header("Follow Settings")]
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector2 minPosition = new Vector2(-10f, -5f);
    [SerializeField] private Vector2 maxPosition = new Vector2(10f, 5f);

    private void Start()
    {
        if (target == null)
        {
            // Try to find the player if target not assigned
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
            else
            {
                Debug.LogError("CameraFollow: No target assigned and no Player tag found!");
            }
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Calculate desired position
        Vector3 desiredPosition = target.position + offset;
        
        // Smoothly move towards target
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        
        // Clamp the camera position within bounds
        smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minPosition.x, maxPosition.x);
        smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minPosition.y, maxPosition.y);
        
        // Apply position
        transform.position = smoothedPosition;
    }
}