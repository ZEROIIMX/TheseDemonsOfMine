using UnityEngine;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject nameInputPanel;

    [Header("Name Input")]
    [SerializeField] private TMP_InputField nameEntryField;

    private void Awake()
    {
        GameManager.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= OnGameStateChanged;
    }

    private void Start()
    {
        OnGameStateChanged(GameManager.Instance.CurrentState);
    }

    private void OnGameStateChanged(GameManager.GameState newState)
    {
        mainMenuPanel.SetActive(newState == GameManager.GameState.MainMenu);
        nameInputPanel.SetActive(newState == GameManager.GameState.NameInput);
    }

    public void OnPlayButtonPressed()
    {
        GameManager.Instance.ShowNameInput();
    }

    public void OnConfirmNameButtonPressed()
    {
        GameManager.Instance.ConfirmNameAndStartGame(nameEntryField.text);
    }
}