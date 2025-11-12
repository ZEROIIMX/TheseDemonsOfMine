using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GameOverUI : MonoBehaviour
{
    [System.Serializable]
    public class ScoreRowUI
    {
        public TextMeshProUGUI rankText;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI scoreText;
        public GameObject rowObject;
    }

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI finalResultText;
    [SerializeField] private List<ScoreRowUI> scoreRows;

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (GameManager.Instance != null)
        {
            if (finalResultText != null)
            {
                string playerName = GameManager.Instance.GetLastName();
                int finalScore = GameManager.Instance.GetLastScore();
                finalResultText.text = $"{playerName}\nScore: {finalScore}";
            }

            DisplayHighScores();
        }
    }

    private void DisplayHighScores()
    {
        HighScores highScores = GameManager.Instance.GetHighScores();

        for (int i = 0; i < scoreRows.Count; i++)
        {
            if (i < highScores.scores.Count)
            {
                scoreRows[i].rowObject.SetActive(true);
                scoreRows[i].rankText.text = (i + 1).ToString();
                scoreRows[i].nameText.text = highScores.scores[i].playerName;
                scoreRows[i].scoreText.text = highScores.scores[i].score.ToString("D6");
            }
            else
            {
                scoreRows[i].rowObject.SetActive(false);
            }
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