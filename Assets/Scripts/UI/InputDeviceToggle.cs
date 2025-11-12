using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputDeviceToggle : MonoBehaviour
{
    [Header("References")]
    public Button leftButton;
    public Button rightButton;
    public TMP_Text deviceLabel; // Text in the middle
    public GameObject controllerImage;
    public GameObject keyboardImage;

    private string[] deviceNames = { "Controller", "Keyboard" };
    private int currentIndex = 0; // 0 = Controller, 1 = Keyboard

    void Start()
    {
        // Load saved setting (default = Controller)
        currentIndex = PlayerPrefs.GetInt("InputDeviceIndex", 0);
        ApplyDevice(currentIndex);

        // Add listeners
        leftButton.onClick.AddListener(ToggleDevice);
        rightButton.onClick.AddListener(ToggleDevice);
    }

    void ToggleDevice()
    {
        // Flip 0 ↔ 1
        currentIndex = 1 - currentIndex;
        ApplyDevice(currentIndex);
    }

    void ApplyDevice(int index)
    {
        bool controllerActive = (index == 0);

        // Toggle images
        controllerImage.SetActive(controllerActive);
        keyboardImage.SetActive(!controllerActive);

        // Update label text
        deviceLabel.text = deviceNames[index];

        // Save preference
        PlayerPrefs.SetInt("InputDeviceIndex", index);
        PlayerPrefs.Save();
    }
}
