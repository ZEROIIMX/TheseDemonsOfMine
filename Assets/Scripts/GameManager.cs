using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState
    {
        MainMenu,
        NameInput,
        Playing,
        Paused,
        GameOver
    }

    public GameState CurrentState { get; private set; }
    private int currentScore;
    private string currentPlayerName;
    private const string HighScoreKey = "HighScores";

    private int lastScore;
    private string lastName;

    public static event Action<GameState> OnGameStateChanged;
    public static event Action<int> OnScoreChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        UpdateGameState(GameState.MainMenu);
    }

    public void UpdateGameState(GameState newState)
    {
        CurrentState = newState;

        switch (CurrentState)
        {
            case GameState.MainMenu:
            case GameState.NameInput:
            case GameState.Paused:
            case GameState.GameOver:
                Time.timeScale = 0f;
                break;
            case GameState.Playing:
                Time.timeScale = 1f;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnGameStateChanged?.Invoke(newState);
    }

    public void PauseGame()
    {
        if (CurrentState != GameState.Playing) return;
        UpdateGameState(GameState.Paused);
        SceneManager.LoadScene("PauseMenu", LoadSceneMode.Additive);
    }

    public void ResumeGame()
    {
        if (CurrentState != GameState.Paused) return;
        SceneManager.UnloadSceneAsync("PauseMenu");
        UpdateGameState(GameState.Playing);
    }

    public void ShowNameInput()
    {
        UpdateGameState(GameState.NameInput);
    }

    public void ConfirmNameAndStartGame(string playerName)
    {
        currentPlayerName = playerName;
        if (string.IsNullOrWhiteSpace(currentPlayerName))
        {
            currentPlayerName = "Player";
        }
        
        SceneManager.LoadScene(1);
        ResetScore();
        UpdateGameState(GameState.Playing);
    }

    public void PlayerDied()
    {
        lastScore = currentScore;
        lastName = currentPlayerName;

        SaveCurrentScore();
        UpdateGameState(GameState.GameOver);
        SceneManager.LoadScene("GameOverScreen");
    }

    private void SaveCurrentScore()
    {
        var newEntry = new HighScoreEntry { playerName = currentPlayerName, score = currentScore };

        string json = PlayerPrefs.GetString(HighScoreKey, "{}");
        HighScores highScores = JsonUtility.FromJson<HighScores>(json);

        if (highScores.scores == null)
        {
            highScores.scores = new List<HighScoreEntry>();
        }

        highScores.scores.Add(newEntry);
        highScores.scores = highScores.scores.OrderByDescending(s => s.score).ToList();

        if (highScores.scores.Count > 10)
        {
            highScores.scores = highScores.scores.Take(10).ToList();
        }

        string updatedJson = JsonUtility.ToJson(highScores);
        PlayerPrefs.SetString(HighScoreKey, updatedJson);
        PlayerPrefs.Save();
    }

    public void AddScore(int points)
    {
        if (CurrentState != GameState.Playing) return;
        currentScore += points;
        OnScoreChanged?.Invoke(currentScore);
    }
    
    private void ResetScore()
    {
        currentScore = 0;
        OnScoreChanged?.Invoke(currentScore);
    }

    public void RetryGame()
    {
        SceneManager.LoadScene(1);
        ResetScore();
        UpdateGameState(GameState.Playing);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
        UpdateGameState(GameState.MainMenu);
    }

    public HighScores GetHighScores()
    {
        string json = PlayerPrefs.GetString(HighScoreKey, "{}");
        HighScores highScores = JsonUtility.FromJson<HighScores>(json);
        if (highScores.scores == null)
        {
            highScores.scores = new List<HighScoreEntry>();
        }
        return highScores;
    }

    public int GetLastScore() => lastScore;
    public string GetLastName() => lastName;
}