using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleTMPTextColor : MonoBehaviour
{
    [Header("Assign your TMP text (UI or 3D)")]
    public TMP_Text targetText; // works for both TextMeshPro and TextMeshProUGUI

    public Color activeColor = Color.white;
    public Color inactiveColor = new Color(1f, 1f, 1f, 0.5f);

    Toggle toggle;

    void Awake()
    {
        toggle = GetComponent<Toggle>();
        if (!targetText) targetText = GetComponentInChildren<TMP_Text>(true);
        toggle.onValueChanged.AddListener(UpdateColor);
        UpdateColor(toggle.isOn);
    }

    void OnEnable() => UpdateColor(toggle.isOn);

    void UpdateColor(bool isOn)
    {
        if (targetText)
            targetText.color = isOn ? activeColor : inactiveColor;
    }
}
