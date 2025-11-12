using UnityEngine;

public class PauseMenuUI : MonoBehaviour
{
    public void OnResumeButtonPressed()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResumeGame();
        }
    }

    public void OnMainMenuButtonPressed()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResumeGame();
            GameManager.Instance.GoToMainMenu();
        }
    }
}