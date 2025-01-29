using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour, IInitializable {

    public static event Action OnSettingsChanged;

    [Header("General")]
    [SerializeField] private Slider cameraShakeSlider;

    [Header("Video")]
    [SerializeField] private Toggle vSyncToggle;
    [SerializeField] private TMP_Dropdown displayModeDropDown;
    [SerializeField] private TMP_Dropdown resolutionDropDown;
    [SerializeField] private TextMeshProUGUI currentResolutionText;
    [SerializeField] private Vector2Int[] resolutions;
    private Vector2Int currentResolution;

    [Header("Audio")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private float minDb;
    [SerializeField] private float maxDb;
    [SerializeField] private Slider SFXVolumeSlider;
    [SerializeField] private Slider UIVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;

    [Serializable]
    public class GameSettings {
        public float CameraShake = 0.5f;

        public bool vSync = true;
        public FullScreenMode fullScreenMode = FullScreenMode.ExclusiveFullScreen;

        public float SFXVolume = 0.5f;
        public float UIVolume = 0.5f;
        public float MusicVolume = 0.5f;
    }

    private static GameSettings currentSettings;
    public static GameSettings GetSettings() {
        return currentSettings;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void Init() {
        currentSettings = new();
    }

    public void Initialize() {
        ScriptInitializer.Instance.StartCoroutine(SetMixerVolumes());
    }

    private IEnumerator SetMixerVolumes() {
        yield return null; // delay to wait for mixers to setup
        audioMixer.SetFloat("SfxVolume", AudioManager.SliderValueToDecibels(currentSettings.SFXVolume));
        audioMixer.SetFloat("UiVolume", AudioManager.SliderValueToDecibels(currentSettings.UIVolume));
        audioMixer.SetFloat("MusicVolume", AudioManager.SliderValueToDecibels(currentSettings.MusicVolume, maxDB: -6f));
    }

    private void OnEnable() {
        UpdateUI();
    }

    private void Update() {
        bool screenResolutionChanged = currentResolution.x != Screen.width || currentResolution.y != Screen.height;
        if (screenResolutionChanged) {
            currentResolutionText.text = $"{Screen.width}x{Screen.height}";

            UpdateResolutionDropdown();
        }
    }

    private void UpdateUI() {
        cameraShakeSlider.value = currentSettings.CameraShake;
        SFXVolumeSlider.value = currentSettings.SFXVolume;
        UIVolumeSlider.value = currentSettings.UIVolume;
        musicVolumeSlider.value = currentSettings.MusicVolume;

        UpdateResolutionDropdown();
    }

    private void UpdateResolutionDropdown() {
        Vector2Int currentResolution = new Vector2Int(Screen.width, Screen.height);

        if (resolutions.Contains(currentResolution)) {
            int resolutionIndex = Array.IndexOf(resolutions, currentResolution);
            resolutionDropDown.value = resolutionIndex;
        }
        else {
            //... make dropdown have "-"
            resolutionDropDown.value = resolutions.Length;
        }
    }

    public void OnCameraShakerChanged(float value) {
        currentSettings.CameraShake = value;
        OnSettingsChanged?.Invoke();
    }

    public void OnVSyncToggled(bool active) {
        QualitySettings.vSyncCount = active ? 1 : 0;
        OnSettingsChanged?.Invoke();
    }

    public void OnScreenModeChanged(int screenModeValue) {

        if (screenModeValue == 0) {
            currentSettings.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        }
        else if (screenModeValue == 1) {
            currentSettings.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
        else if (screenModeValue == 2) {
            currentSettings.fullScreenMode = FullScreenMode.Windowed;
        }

        Screen.fullScreenMode = currentSettings.fullScreenMode;

        OnSettingsChanged?.Invoke();
    }

    public void OnResolutionChanged(int dropdownValue) {

        bool valueOutsideRange = dropdownValue > resolutions.Length - 1;
        if (valueOutsideRange) {
            return;
        }

        Vector2Int selectedResolution = resolutions[dropdownValue];
        Screen.SetResolution(selectedResolution.x, selectedResolution.y, currentSettings.fullScreenMode);

        OnSettingsChanged?.Invoke();
    }

    // dont set audioMixer of sfx yet because sfx should be quiet when ui is open, the sfx volume is set in audiomanager
    public void OnSFXVolumeSliderChanged(float value) {
        currentSettings.SFXVolume = value;
        OnSettingsChanged?.Invoke();
    }

    public void OnUIVolumeSliderChanged(float value) {
        currentSettings.UIVolume = value;
        audioMixer.SetFloat("UiVolume", AudioManager.SliderValueToDecibels(value));
        OnSettingsChanged?.Invoke();
    }

    public void OnMusicVolumeSliderChanged(float value) {
        currentSettings.MusicVolume = value;
        //... make max db -6 to make music quieter
        audioMixer.SetFloat("MusicVolume", AudioManager.SliderValueToDecibels(value, maxDB: -6f));
        OnSettingsChanged?.Invoke();
    }

    public void ResetToDefaults() {
        currentSettings = new GameSettings();
        UpdateUI();

        OnSettingsChanged?.Invoke();
    }
}
