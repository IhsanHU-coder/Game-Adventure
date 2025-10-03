using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public void StartGame()
    {
        GameManager.Instance.ResetPlayerHealth();
        SceneManager.LoadScene("GameScene");
    }

    public void BackToMenu()
    {
        GameManager.Instance.ResetPlayerHealth();
        SceneManager.LoadScene("MainMenu");
    }

    public void AfterWin()
    {
        SceneManager.LoadScene("MainMenu");
    }


}
