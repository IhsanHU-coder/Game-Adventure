using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [Header("Parallax Settings")]
    [Range(0f, 1f)]
    public float parallaxX = 0.5f;
    [Range(0f, 1f)]
    public float parallaxY = 0.3f;

    private Transform cameraTransform;
    private Vector3 lastCameraPosition;

    void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;
    }

    void LateUpdate()
    {
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;

        // Move background slower than camera
        transform.position += new Vector3(
            deltaMovement.x * parallaxX,
            deltaMovement.y * parallaxY,
            0);

        lastCameraPosition = cameraTransform.position;
    }
}
