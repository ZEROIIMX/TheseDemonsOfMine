using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDSizeToggle : MonoBehaviour
{
    [Header("References")]
    public RectTransform hudRoot;     // The parent RectTransform of your HUD
    public Button leftButton;         // Left arrow button
    public Button rightButton;        // Right arrow button
    public TMP_Text sizeLabel;            // The text displaying current size

    private string[] sizeNames = { "Small", "Medium", "Large" };
    private float[] scaleValues = { 0.6f, 0.8f, 1.0f };
    private int currentIndex = 1; // Default: Medium

    void Start()
    {
        // Load saved index (default = 1)
        currentIndex = PlayerPrefs.GetInt("HUDSizeIndex", 1);
        ApplyHUDSize(currentIndex);

        // Add button listeners
        leftButton.onClick.AddListener(PreviousSize);
        rightButton.onClick.AddListener(NextSize);
    }

    void PreviousSize()
    {
        currentIndex--;
        if (currentIndex < 0) currentIndex = sizeNames.Length - 1; // Wrap around
        ApplyHUDSize(currentIndex);
    }

    void NextSize()
    {
        currentIndex++;
        if (currentIndex >= sizeNames.Length) currentIndex = 0; // Wrap around
        ApplyHUDSize(currentIndex);
    }

    void ApplyHUDSize(int index)
    {
        if (hudRoot != null)
        {
            hudRoot.localScale = Vector3.one * scaleValues[index];
        }

        sizeLabel.text = sizeNames[index];
        PlayerPrefs.SetInt("HUDSizeIndex", index);
        PlayerPrefs.Save();
    }
}
