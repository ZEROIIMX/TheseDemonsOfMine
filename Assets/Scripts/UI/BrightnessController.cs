using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class BrightnessController : MonoBehaviour
{
    [Header("References")]
    public Volume globalVolume;      // Assign your Global Volume
    public Slider brightnessSlider;  // Assign your UI Slider

    private ColorAdjustments colorAdjustments;

    private const float minExposure = -2f;
    private const float maxExposure = 2f;

    void Start()
    {
        // Get the ColorAdjustments override from the Volume
        if (globalVolume.profile.TryGet(out colorAdjustments))
        {
            // Load saved brightness if available
            float savedValue = PlayerPrefs.GetFloat("Brightness", 0.5f);
            brightnessSlider.value = savedValue;

            UpdateBrightness(savedValue);

            // Connect slider to update function
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
            colorAdjustments.postExposure.value = Mathf.Lerp(minExposure, maxExposure, value);
            PlayerPrefs.SetFloat("Brightness", value); // Save setting
        }

        PlayerPrefs.Save();
    }
}
