using UnityEngine;
using UnityEngine.SceneManagement;

public class changeScene : MonoBehaviour
{

    public void StartGame()
    {
        // Ganti "GameScene" dengan nama scene yang ingin dimuat
        SceneManager.LoadScene("GameScene");
    }

    // Opsional: fungsi untuk keluar dari game
    public void QuitGame()
    {
        Debug.Log("Keluar dari game...");
        Application.Quit();
    }
}
