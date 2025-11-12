using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer volumeMixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [SerializeField] private TextMeshProUGUI masterText;
    [SerializeField] private TextMeshProUGUI musicText;
    [SerializeField] private TextMeshProUGUI sfxText;

    private void Start()
    {
        LoadVolume();

        // Koppla event så att både ljud och text uppdateras live
        masterSlider.onValueChanged.AddListener(_ => SetMasterVolume());
        musicSlider.onValueChanged.AddListener(_ => SetMusicVolume());
        sfxSlider.onValueChanged.AddListener(_ => SetSfxVolume());
    }

    private float LinearToDb(float v) => (v <= 0.0001f) ? -80f : 20f * Mathf.Log10(v);

    public void SetMasterVolume()
    {
        float volume = masterSlider.value;
        volumeMixer.SetFloat("MasterVol", LinearToDb(volume));
        PlayerPrefs.SetFloat("masterVolume", volume);
        masterText.text = Mathf.RoundToInt(volume * 100).ToString();
    }

    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        volumeMixer.SetFloat("MusicVol", LinearToDb(volume));
        PlayerPrefs.SetFloat("musicVolume", volume);
        musicText.text = Mathf.RoundToInt(volume * 100).ToString();
    }

    public void SetSfxVolume()
    {
        float volume = sfxSlider.value;
        volumeMixer.SetFloat("SfxVol", LinearToDb(volume));
        PlayerPrefs.SetFloat("sfxVolume", volume);
        sfxText.text = Mathf.RoundToInt(volume * 100).ToString();
    }

    private void LoadVolume()
    {
        masterSlider.value = PlayerPrefs.GetFloat("masterVolume", 1f);
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume", 1f);

        SetMasterVolume();
        SetMusicVolume();
        SetSfxVolume();
    }
}
