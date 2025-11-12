using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        GameOver
    }

    private GameState currentState;

    [Header("UI Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject pauseMenuPanel;

    public static event Action<GameState> OnGameStateChanged;

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
        currentState = newState;

        mainMenuPanel?.SetActive(false);
        gameOverPanel?.SetActive(false);
        pauseMenuPanel?.SetActive(false);

        switch (currentState)
        {
            case GameState.MainMenu:
                Time.timeScale = 0f;
                mainMenuPanel?.SetActive(true);
                break;
            case GameState.Playing:
                Time.timeScale = 1f;
                break;
            case GameState.Paused:
                Time.timeScale = 0f;
                pauseMenuPanel?.SetActive(true);
                break;
            case GameState.GameOver:
                Time.timeScale = 0f;
                gameOverPanel?.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnGameStateChanged?.Invoke(newState);
    }

    public void StartGame()
    {
        UpdateGameState(GameState.Playing);
    }

    public void PlayerDied()
    {
        UpdateGameState(GameState.GameOver);
    }
}