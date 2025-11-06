using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleSceneManager : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ReloadCurrent()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}