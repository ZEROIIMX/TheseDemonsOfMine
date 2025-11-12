using UnityEngine;
using TMPro;

public class InGameUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI scoreText;

    private void Awake()
    {
        GameManager.OnGameStateChanged += OnGameStateChanged;
        GameManager.OnScoreChanged += OnScoreChanged;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= OnGameStateChanged;
        GameManager.OnScoreChanged -= OnScoreChanged;
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            OnGameStateChanged(GameManager.Instance.CurrentState);
            OnScoreChanged(0);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameManager.Instance.CurrentState == GameManager.GameState.Playing)
            {
                GameManager.Instance.PauseGame();
            }
            else if (GameManager.Instance.CurrentState == GameManager.GameState.Paused)
            {
                GameManager.Instance.ResumeGame();
            }
        }
    }

    private void OnGameStateChanged(GameManager.GameState newState)
    {
        scoreText?.gameObject.SetActive(newState == GameManager.GameState.Playing || newState == GameManager.GameState.Paused);
    }

    private void OnScoreChanged(int newScore)
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + newScore;
        }
    }
}