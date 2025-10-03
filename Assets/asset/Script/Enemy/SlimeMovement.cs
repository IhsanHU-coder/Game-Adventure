using UnityEngine;
using System.Collections;

public class SlimeMovement : MonoBehaviour
{
    [Header("Slime Movement")]
    public Transform target;
    public float speed = 2f;
    public float chaseRange = 5f;
    private bool isFacingRight = true;

    [Header("Attack Collider")]
    [SerializeField] private BoxCollider2D attackCollider;
    public float attackRange = 2.2f;

    [Header("Utilities")]
    [SerializeField] private Animator animator;
    public HealthManager healthSlime;

    private void Update()
    {
        float distance = Vector2.Distance(transform.position, target.position);

        FaceTarget();

        if (distance <= attackRange)
        {
            animator.SetBool("isAttacking", true);
            animator.SetBool("isRunning", false);
        }
        else if (distance <= chaseRange)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            transform.position += (Vector3)direction * speed * Time.deltaTime;

            animator.SetBool("isRunning", true);
            animator.SetBool("isAttacking", false);
        }
        else
        {
            animator.SetBool("isRunning", false);
            animator.SetBool("isAttacking", false);
        }

        if (attackCollider != null)
            attackCollider.enabled = false;
    }

    private void FaceTarget()
    {
        if (target.position.x > transform.position.x && isFacingRight)
        {
            Flip();
        }
        else if (target.position.x < transform.position.x && !isFacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // Fungsi dipanggil dari animation event saat animasi attack
    public void TriggerAttackCollider()
    {
        StartCoroutine(EnableColliderWithDelay());
    }

    private IEnumerator EnableColliderWithDelay()
    {
        if (attackCollider != null)
        {
            attackCollider.enabled = true;
            yield return new WaitForSeconds(0.1f); // aktif singkat
            attackCollider.enabled = false;
        }
    }

    // Slime menyerang Player jika player kena collider
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (healthSlime != null)
            {
                healthSlime.TakeDamage(20f); // Player kena damage
            }
        }
    }

    // Slime kena serangan dari Player
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("damage"))
        {
            healthSlime.TakeDamageEnemySlime(25f); // Slime kena serangan
            animator.SetFloat("Health", healthSlime.GetHealth());
            animator.SetTrigger("hit");

            if (healthSlime.GetHealth() <= 0)
            {
                speed = 0f;
                animator.SetFloat("Health", healthSlime.GetHealth());
                StartCoroutine(WaitAndDestroy());
            }
        }
    }

    private IEnumerator WaitAndDestroy()
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("SlimeDie"))
        {
            yield return null ;
        }

        float dieAnimDuration = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(dieAnimDuration);

        Destroy(gameObject);
    }
}
