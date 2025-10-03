using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [Header("Sistem Input")]
    private Rigidbody2D rb;

    [Header("Gerakan Karakter")]
    [SerializeField] private float moveSpeed = 5f; // Kecepatan lari
    private float moveInput;

    [Header("Karakter Mati")]
    [SerializeField] float knockForce = 5.0f;
    [SerializeField] BoxCollider2D stand;
    [SerializeField] BoxCollider2D die;


    

    [Header("Lompatan Karakter")]
    [SerializeField] private float jumpForce = 7f; // Kekuatan lompat
    [SerializeField] private Transform groundCheck; // Titik untuk cek tanah
    [SerializeField] private float groundCheckRadius = 0.2f; // Radius cek tanah
    [SerializeField] private LayerMask groundLayer; // Layer yang dianggap sebagai tanah
    private bool isGrounded = false; // Apakah menyentuh tanah

    [Header("Serangan Karakter")]
    [SerializeField] private float attackCooldown = 0.5f; // Waktu jeda antara serangan
    private bool isAttacking = false;
    private float attackTimer = 0f;

    [Header("Hitbox Serangan")]
    [SerializeField] private BoxCollider2D attackHitbox; // Collider untuk mendeteksi serangan
    [SerializeField] private float hitboxMoveDistance = 1.5f; // Jarak gerakan hitbox
    [SerializeField] private float hitboxSpeed = 10f; // Kecepatan gerakan hitbox
    [SerializeField] private float knockbackForce = 5f; // Gaya dorong ke musuh

    [Header("Utilitas")]
    public Animator animator; // Untuk mengontrol animasi
    public HealthManager health;
    [SerializeField] GameObject backgroundCave;
    [SerializeField] GameObject nextStage;
    [SerializeField] GameObject backStage;
    [SerializeField] GameObject outdor;
    [SerializeField] GameObject indor;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        moveSpeed = 5f;
        stand.enabled = true;
        die.enabled = false;
        backStage.SetActive(false);
        outdor.SetActive(true);
        indor.SetActive(false);
        health.GetHealth();
    }

    void Update()
    {

        animator.SetInteger("Health", (int)health.GetHealth());

        // Input gerakan horizontal
        moveInput = Input.GetAxisRaw("Horizontal");

        // Animasi lari
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow))
        {
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

        // Logika serangan dengan cooldown
        if (Input.GetKeyDown(KeyCode.E) && !isAttacking)
        {
            Attack();
        }

        // Timer cooldown serangan
        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                isAttacking = false;
                animator.SetBool("isAttacking", false);
            }
        }

        // Membalik arah seluruh objek, bukan hanya sprite
        if (moveInput > 0 && transform.localScale.x < 0)
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
        else if (moveInput < 0 && transform.localScale.x > 0)
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }

        // Cek apakah menyentuh tanah
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        animator.SetBool("isJumping", !isGrounded);

        // Lompat saat menekan tombol dan menyentuh tanah
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow)) && isGrounded)
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        // Gerakan horizontal karakter
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    private void Jump()
    {
        // Memberi gaya lompat
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        animator.SetBool("isJumping", true);
        isGrounded = false;
    }

    private void Attack()
    {
        // Mengaktifkan serangan
        isAttacking = true;
        attackTimer = attackCooldown;
        animator.SetBool("isAttacking", true);

        // Mulai serangan (aktifkan hitbox)
        StartCoroutine(PerformAttackHitbox());
    }

    public IEnumerator PerformAttackHitbox()
    {
        // Posisikan hitbox di depan karakter
        float direction = transform.localScale.x > 0 ? 1f : -1f;
        Vector3 startPosition = transform.position + new Vector3(direction * 1f, 0, 0);
        Vector3 endPosition = transform.position + new Vector3(direction * hitboxMoveDistance, 0, 0);

        attackHitbox.transform.position = startPosition;
        attackHitbox.enabled = true;

        float elapsed = 0f;
        float duration = 0.5f; // Durasi gerakan hitbox

        // Gerakkan hitbox secara perlahan
        while (elapsed < duration)
        {
            attackHitbox.transform.position = Vector3.Lerp(startPosition, endPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Nonaktifkan hitbox setelah selesai
        attackHitbox.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Jika hitbox aktif dan menyentuh musuh
        if (attackHitbox.enabled && collision.CompareTag("Enemy"))
        {
            Rigidbody2D enemyRb = collision.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                // Tambahkan knockback ke musuh
                Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;
                enemyRb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
            }
        }

        if (collision.CompareTag("nextStage"))
        {
            backgroundCave.SetActive(false);
            backStage.SetActive(true);
            indor.SetActive(true);
            outdor.SetActive(false);
        }

        if (collision.CompareTag("backStage"))
        {
            backgroundCave.SetActive(true);
            backStage.SetActive(false);
            indor.SetActive(false);
            outdor.SetActive(true);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("damageMushroom"))
        {
            
            StartCoroutine(waitTakeDamage());

            Vector2 knockbackDir = (transform.position - collision.transform.position).normalized;
            rb.AddForce(knockbackDir * knockForce, ForceMode2D.Impulse);

           
            if (health.GetHealth() <= 0)
            {
                moveSpeed = 0;
                stand.enabled = false;
                die.enabled = true;
                StartCoroutine(waitChangeScene());
            }
        }

        //Mendeteksi Collider
        if (collision.collider.CompareTag("damageBoss"))
        {

            StartCoroutine(waitTakeBossDamage());

            Vector2 knockbackDir = (transform.position - collision.transform.position).normalized;
            rb.AddForce(knockbackDir * knockForce, ForceMode2D.Impulse);


            if (health.GetHealth() <= 0)
            {
                moveSpeed = 0;
                stand.enabled = false;
                die.enabled = true;
                StartCoroutine(waitChangeScene());
            }
        }

    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("door"))
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                print("Anda Berhasil Memasuki Tempat Boss");
                SceneManager.LoadScene("Game2");
            }
        }
    }



    public IEnumerator waitTakeDamage()
    {
        health.TakeDamage(25f);
        animator.SetInteger("Health", (int)health.GetHealth());
        yield return new WaitForSeconds(0.5f);
    }

    //Mengurahi Darah Player
    public IEnumerator waitTakeBossDamage()
    {
        health.TakeDamage(50f);
        animator.SetInteger("Health", (int)health.GetHealth());
        yield return new WaitForSeconds(0.5f);
    }

    public IEnumerator waitChangeScene()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("GameOver");
    }



}
