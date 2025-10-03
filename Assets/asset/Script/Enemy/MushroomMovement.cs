using UnityEngine;
using System.Collections;

public class MushroomMovement : MonoBehaviour
{
    [Header("Mushroom Attack")]
    [SerializeField] private float attackRange = 2.1f;

    [Header("Mushroom Die")]
    public int health = 100;
    [SerializeField] BoxCollider2D stand;
    [SerializeField] BoxCollider2D die;


    [Header("Utilities")]
    [SerializeField] private Animator animator;
    public HealthManager healthMushroom;

    [Header("Attack Collider")]
    [SerializeField] private BoxCollider2D attackCollider;

    [Header("Mushroom Run")]
    public Transform target;
    public float speed = 2f;
    public float chaseRange = 10f;
    private bool isFacingRight = true;

    private void Start()
    {
        stand.enabled = true;
        die.enabled = false;
        healthMushroom.GetHealth();
    }

    void Update()
    {
        float distance = Vector2.Distance(transform.position, target.position);
        FaceTarget();

        if (distance < attackRange)
        {
            animator.SetBool("isAttacking", true);
            animator.SetBool("isRunning", false);
            return;
        }

        if (distance < chaseRange)
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
            //FlipHealth();
        }
        else if (target.position.x < transform.position.x && !isFacingRight)
        {
            Flip();
            //FlipHealth();
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    //private void FlipHealth()
    //{
    //    isFacingRight = isFacingRight;
    //    Vector3 scale = healthMushroom.healthBarEnemy.transform.localScale;
    //    scale.x *= -1;
    //    healthMushroom.healthBarEnemy.transform.localScale = scale;
    //}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("damage"))
        {
            healthMushroom.TakeDamageEnemy(50f);
            animator.SetInteger("Health", (int)healthMushroom.GetHealth());
            animator.SetTrigger("hit");
            StartCoroutine(WaitGetHit());
            speed = 2f;
            

            if (healthMushroom.GetHealth() <= 0)
            {
                speed = 0;
                stand.enabled = false;
                die.enabled = true;
                StartCoroutine(WaitAndDestroy());
            }
        }
    }

    private IEnumerator WaitGetHit()
    {
        speed = 0f;
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("MushroomHit"))
        {
            yield return null;
        }

        float dieAnimDuration = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(dieAnimDuration);
        animator.SetTrigger("hit");
        //animator.SetTrigger("hit");

    }

    private IEnumerator WaitAndDestroy()
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("MushroomDie"))
        {
            yield return null;
        }

        float dieAnimDuration = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(dieAnimDuration);

        Destroy(gameObject);
    }

    private IEnumerator EnableColliderWithDelay()
    {
        //yield return new WaitForSeconds(0.05f); // sedikit delay jika perlu
        if (attackCollider != null)
        {
            attackCollider.enabled = true;
            yield return new WaitForSeconds(0.05f); // aktif selama 0.1 detik
            attackCollider.enabled = false;
        }
    }

    //  Fungsi ini dipanggil dari animation event pada 0.3 detik animasi attack
    public void TriggerAttackCollider()
    {
        StartCoroutine(EnableColliderWithDelay());
    }
}
