using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public void LoadScene1()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }
    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void Quit()
    {
        Application.Quit();
    }


}
