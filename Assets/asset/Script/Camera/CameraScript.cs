using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform target;               // Who the camera should follow
    public Vector3 offset = new Vector3(0f, 2f, -10f);  // Offset from the target

    public float followSpeed = 5f;         // How fast the camera follows

    public Vector2 minLimit = new Vector2(-10f, -5f);  // Camera boundary min
    public Vector2 maxLimit = new Vector2(10f, 5f);    // Camera boundary max

    void Start()
    {
        // If no target is set, try to find a GameObject tagged "Player"
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
            else
            {
                Debug.LogError("CameraFollow: No target assigned and no object with tag 'Player' found.");
            }
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Target position with offset
        Vector3 targetPosition = target.position + offset;

        // Smooth movement towards the target
        Vector3 newPosition = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        // Clamp the position so the camera doesn't go out of bounds
        newPosition.x = Mathf.Clamp(newPosition.x, minLimit.x, maxLimit.x);
        newPosition.y = Mathf.Clamp(newPosition.y, minLimit.y, maxLimit.y);

        // Update camera position
        transform.position = newPosition;
    }
}
