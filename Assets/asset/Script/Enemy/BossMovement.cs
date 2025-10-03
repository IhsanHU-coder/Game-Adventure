using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BossMovement : MonoBehaviour
{
    [Header("Boss Movement")]
    public Transform target;
    public float speed = 5f;
    public float attackRange = 2.5f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Attack Collider")]
    [SerializeField] private BoxCollider2D attackCollider;

    private bool isFacingRight = true;

    public HealthManager healthBoss;

    private void Start()
    {
        StartCoroutine(BossLoop());
    }

    private void Update()
    {
        FaceTarget();
    }

    IEnumerator BossLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);

            while (Vector2.Distance(transform.position, target.position) > attackRange)
            {
                Vector2 direction = (target.position - transform.position).normalized;
                transform.position += (Vector3)direction * speed * Time.deltaTime;

                if (animator != null)
                    animator.SetBool("isRunning", true);

                yield return null;
            }

            if (animator != null)
                animator.SetBool("isRunning", false);

            if (animator != null)
                animator.SetBool("isAttacking", true);

            yield return new WaitForSeconds(0.8f);

            if (animator != null)
                animator.SetBool("isAttacking", false);

            yield return new WaitForSeconds(5f);
        }
    }


    private void FaceTarget()
    {
        if (target == null) return;

        if (target.position.x > transform.position.x && !isFacingRight)
        {
            Flip();
        }
        else if (target.position.x < transform.position.x && isFacingRight)
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

    public void TriggerAttackCollider()
    {
        StartCoroutine(EnableAttackCollider());
    }

    private IEnumerator EnableAttackCollider()
    {
        if (attackCollider != null)
        {
            attackCollider.enabled = true;
            yield return new WaitForSeconds(0.05f); // durasi aktif
            attackCollider.enabled = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("damage"))
        {
            healthBoss.TakeDamageEnemyBoss(10f);
            animator.SetInteger("Health", (int)healthBoss.GetHealth());
            animator.SetTrigger("hit");
            StartCoroutine(WaitGetHit());
            speed = 5;


            if (healthBoss.GetHealth() <= 0)
            {
                speed = 0;
                SceneManager.LoadScene("Win");
                StartCoroutine(WaitAndDestroy());
                
            }
        }
    }

    private IEnumerator WaitGetHit()
    {
        speed = 0f;
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("BossHit"))
        {
            yield return null;
        }

        float dieAnimDuration = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(dieAnimDuration);
        

    }

    private IEnumerator WaitAndDestroy()
    {
        speed = 0;
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("BossDie"))
        {
            yield return null;
        }

        float dieAnimDuration = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(dieAnimDuration);

        Destroy(gameObject);
    }
}
