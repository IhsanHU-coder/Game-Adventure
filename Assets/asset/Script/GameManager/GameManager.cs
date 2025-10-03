using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public float playerHealth = 100f;
    public float maxPlayerHealth = 100f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ResetPlayerHealth()
    {
        playerHealth = maxPlayerHealth;
    }
}
