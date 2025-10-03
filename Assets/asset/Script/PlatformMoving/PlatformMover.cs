using UnityEngine;

public class PlatformMover : MonoBehaviour
{
    [Header("Movement Points")]
    public Transform pointA;
    public Transform pointB;

    [Header("Settings")]
    public float speed = 2f;
    private Vector3 target;

    void Start()
    {
        // dimulai dengan menuju point B
        target = pointB.position;
    }

    void Update()
    {
        // jalan menuju target yang sudah ditentukan
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // berubah target ketika sudah benar benar mendekati
        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            if (target == pointB.position)
                target = pointA.position;
            else
                target = pointB.position;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.collider.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.collider.transform.SetParent(null);
        }
    }

}
