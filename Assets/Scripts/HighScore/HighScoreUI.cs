using UnityEngine;
using TMPro;
using System.Text;

public class HighScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI highScoreDisplayText;

    void Start()
    {
        DisplayHighScores();
    }

    private void DisplayHighScores()
    {
        if (GameManager.Instance == null || highScoreDisplayText == null)
        {
            return;
        }

        HighScores highScores = GameManager.Instance.GetHighScores();
        StringBuilder sb = new StringBuilder();

        if (highScores.scores.Count == 0)
        {
            sb.AppendLine("No high scores yet!");
        }
        else
        {
            for (int i = 0; i < highScores.scores.Count; i++)
            {
                sb.AppendLine($"{i + 1}. {highScores.scores[i].playerName} - {highScores.scores[i].score}");
            }
        }

        highScoreDisplayText.text = sb.ToString();
    }
}