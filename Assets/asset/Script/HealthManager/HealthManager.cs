using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HealthManager : MonoBehaviour
{
    public Image healthBar;
    public Image healthBarEnemyMushroom;
    public Image healthBarEnemySlime;
    public Image healthBarEnemyBoss;
    public float health = 100f;
    public float maxHealth = 100f;

    void Start()
    {
        // Ambil data dari GameManager
        health = GameManager.Instance.playerHealth;
        maxHealth = GameManager.Instance.maxPlayerHealth;

        UpdateHealthUI();
    }

    public void TakeDamageEnemyBoss(float amount)
    {
        health -= amount;
        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthUIEnemyBoss();
    }
    public void UpdateHealthUIEnemyBoss()
    {
        if (healthBarEnemyBoss != null)
        {
            healthBarEnemyBoss.fillAmount = health / maxHealth;
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        health = Mathf.Clamp(health, 0, maxHealth);

        // Simpan kembali ke GameManager
        GameManager.Instance.playerHealth = health;

        UpdateHealthUI();
    }
    public void UpdateHealthUI()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = health / maxHealth;
        }
    }

    public void TakeDamageEnemy(float amount)
    {
        health -= amount;
        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthUIEnemy();
    }
    public void UpdateHealthUIEnemy()
    {
        if (healthBarEnemyMushroom != null)
        {
            healthBarEnemyMushroom.fillAmount = health / maxHealth;
        }
    }

    public void TakeDamageEnemySlime(float amount)
    {
        health -= amount;
        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthUIEnemySlime();
    }
    public void UpdateHealthUIEnemySlime()
    {
        if (healthBarEnemySlime != null)
        {
            healthBarEnemySlime.fillAmount = health / maxHealth;
        }
    }


    public float GetHealth()
    {
        return health;
    }
}
