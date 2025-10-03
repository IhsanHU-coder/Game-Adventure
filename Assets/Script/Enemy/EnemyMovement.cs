using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [Header("Utilities")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    public Transform target;       // Objek pemain (player)
    public float speed = 2f;       // Kecepatan musuh
    public float chaseRange = 5f;  // Jarak deteksi pemain

    void Update()
    {
        // Hitung jarak antara musuh dan target
        float distance = Vector2.Distance(transform.position, target.position);

        // Jika pemain berada dalam jarak chaseRange
        if (distance < chaseRange)
        {
            // Hitung arah ke pemain
            Vector2 direction = (target.position - transform.position).normalized;

            // Gerakkan musuh ke arah pemain
            transform.position += (Vector3)direction * speed * Time.deltaTime;
        }
        
    }

}
