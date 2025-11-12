using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using TMPro;

public class BrightnessController : MonoBehaviour
{
    [Header("References")]
    public Volume globalVolume;         // Global Volume i scenen
    public Slider brightnessSlider;     // UI-slider för ljusstyrka
    public TextMeshProUGUI brightnessText; // Text som visar värdet

    private ColorAdjustments colorAdjustments;

    private const float minExposure = -2f;
    private const float maxExposure = 2f;

    private void Start()
    {
        if (globalVolume.profile.TryGet(out colorAdjustments))
        {
            float savedValue = PlayerPrefs.GetFloat("Brightness", 0.5f);
            brightnessSlider.value = savedValue;

            UpdateBrightness(savedValue);

            brightnessSlider.onValueChanged.AddListener(UpdateBrightness);
        }
        else
        {
            Debug.LogError("ColorAdjustments not found in the Volume Profile!");
        }
    }

    public void UpdateBrightness(float value)
    {
        if (colorAdjustments != null)
        {
            // Omvandla slider (0–1) till -2 till +2 exposure
            colorAdjustments.postExposure.value = Mathf.Lerp(minExposure, maxExposure, value);
            PlayerPrefs.SetFloat("Brightness", value);
        }

        // Visa som procent
        if (brightnessText != null)
        {
            brightnessText.text = $"{Mathf.RoundToInt(value * 100)}";
            // Alternativt visa det faktiska exposurevärdet:
            // brightnessText.text = $"{colorAdjustments.postExposure.value:0.00}";
        }

        PlayerPrefs.Save();
    }
}
