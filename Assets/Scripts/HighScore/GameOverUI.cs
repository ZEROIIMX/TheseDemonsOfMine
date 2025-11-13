using UnityEngine;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI finalResultText;

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (GameManager.Instance != null && finalResultText != null)
        {
            string playerName = GameManager.Instance.GetLastName();
            int finalScore = GameManager.Instance.GetLastScore();
            finalResultText.text = $"{playerName}\nScore: {finalScore}";
        }
    }

    public void OnRetryButtonPressed()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RetryGame();
        }
    }

    public void OnMainMenuButtonPressed()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GoToMainMenu();
        }
    }
}