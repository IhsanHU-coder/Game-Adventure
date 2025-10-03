using UnityEngine;

public class ParallaxBackground2 : MonoBehaviour
{
    [Header("Parallax Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float parallaxEffectX = 0.5f;
    [Range(0f, 1f)]
    [SerializeField] private float parallaxEffectY = 0.3f;
    [SerializeField] private bool showDebugGizmos = true;

    private float lengthX;
    private float startPositionX;
    private float startPositionY;
    private Transform cameraTransform;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (spriteRenderer == null)
        {
            Debug.LogError($"No SpriteRenderer found on {gameObject.name}!");
            enabled = false;
            return;
        }

        // Store initial positions and size
        startPositionX = transform.position.x;
        startPositionY = transform.position.y;
        lengthX = spriteRenderer.bounds.size.x;

        // Ensure background is behind other elements
        spriteRenderer.sortingLayerName = "Background";
    }

    private void LateUpdate()
    {
        if (cameraTransform == null) return;

        // Calculate parallax positions
        float parallaxX = (startPositionX - cameraTransform.position.x) * parallaxEffectX;
        float parallaxY = (startPositionY - cameraTransform.position.y) * parallaxEffectY;

        // Apply parallax effect
        Vector3 newPosition = new Vector3(
            cameraTransform.position.x + parallaxX,
            cameraTransform.position.y + parallaxY,
            transform.position.z
        );

        // Update position
        transform.position = newPosition;

        // Draw debug lines in scene view
        if (showDebugGizmos)
        {
            Debug.DrawLine(transform.position, transform.position + Vector3.right * lengthX, Color.yellow);
            Debug.DrawLine(transform.position, transform.position + Vector3.up * spriteRenderer.bounds.size.y, Color.cyan);
        }
    }

    private void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;

        // Always draw gizmos (not just when selected)
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            // Draw background bounds
            Gizmos.color = new Color(1f, 1f, 0f, 0.5f); // Semi-transparent yellow
            Gizmos.DrawWireCube(transform.position, sr.bounds.size);

            // Draw movement range
            Gizmos.color = new Color(0f, 1f, 1f, 0.3f); // Semi-transparent cyan
            float maxParallaxX = sr.bounds.size.x * parallaxEffectX;
            float maxParallaxY = sr.bounds.size.y * parallaxEffectY;
            Gizmos.DrawWireCube(
                transform.position,
                new Vector3(sr.bounds.size.x + maxParallaxX * 2, 
                           sr.bounds.size.y + maxParallaxY * 2, 
                           1)
            );
        }
    }
}