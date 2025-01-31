using QFSW.QC.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

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
        public FullScreenMode FullScreenMode = FullScreenMode.ExclusiveFullScreen;
        public Vector2Int Resolution = new Vector2Int(1920, 1080);

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
        LoadSettings();
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
    private void OnDisable() {
        SaveSettings();
    }

    private void Update() {
        bool screenResolutionChanged = currentSettings.Resolution.x != Screen.width || currentSettings.Resolution.y != Screen.height;
        if (screenResolutionChanged) {
            currentSettings.Resolution = new(Screen.width, Screen.height);
            currentResolutionText.text = $"{Screen.width}x{Screen.height}";

            UpdateResolutionDropdown();
        }
    }

    private void UpdateUI() {
        cameraShakeSlider.value = currentSettings.CameraShake;

        vSyncToggle.isOn = currentSettings.vSync;

        SFXVolumeSlider.value = currentSettings.SFXVolume;
        UIVolumeSlider.value = currentSettings.UIVolume;
        musicVolumeSlider.value = currentSettings.MusicVolume;

        UpdateFullScreenModeDropdown();
        UpdateResolutionDropdown();
    }

    private void UpdateFullScreenModeDropdown() {
        if (currentSettings.FullScreenMode == FullScreenMode.ExclusiveFullScreen) {
            displayModeDropDown.value = 0;
        }
        else if (currentSettings.FullScreenMode == FullScreenMode.FullScreenWindow) {
            displayModeDropDown.value = 1;
        }
        else if (currentSettings.FullScreenMode == FullScreenMode.Windowed) {
            displayModeDropDown.value = 2;
        }
    }

    private void UpdateResolutionDropdown() {
        if (resolutions.Contains(currentSettings.Resolution)) {
            int resolutionIndex = Array.IndexOf(resolutions, currentSettings.Resolution);
            resolutionDropDown.value = resolutionIndex;
        }
        else {
            //... make dropdown have "-"
            resolutionDropDown.value = resolutions.Length;
        }
    }

    #region On Settings Changed Methods

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
            currentSettings.FullScreenMode = FullScreenMode.ExclusiveFullScreen;
        }
        else if (screenModeValue == 1) {
            currentSettings.FullScreenMode = FullScreenMode.FullScreenWindow;
        }
        else if (screenModeValue == 2) {
            currentSettings.FullScreenMode = FullScreenMode.Windowed;
        }

        Screen.fullScreenMode = currentSettings.FullScreenMode;

        OnSettingsChanged?.Invoke();
    }

    public void OnResolutionChanged(int dropdownValue) {

        bool valueOutsideRange = dropdownValue > resolutions.Length - 1;
        if (valueOutsideRange) {
            return;
        }

        Screen.SetResolution(resolutions[dropdownValue].x, resolutions[dropdownValue].y, currentSettings.FullScreenMode);

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

        QualitySettings.vSyncCount = currentSettings.vSync ? 1 : 0;
        Screen.SetResolution(currentSettings.Resolution.x, currentSettings.Resolution.y, currentSettings.FullScreenMode);

        audioMixer.SetFloat("UiVolume", AudioManager.SliderValueToDecibels(currentSettings.UIVolume));
        audioMixer.SetFloat("MusicVolume", AudioManager.SliderValueToDecibels(currentSettings.MusicVolume, maxDB: -6f));

        UpdateUI();

        OnSettingsChanged?.Invoke();
    }

    #endregion

    #region Save and Load Settings

    private void SaveSettings() {
        PlayerPrefs.SetFloat("CameraShake", currentSettings.CameraShake);

        PlayerPrefs.SetInt("vSync", currentSettings.vSync ? 1 : 0); // use 0 as false and 1 as true
        PlayerPrefs.SetInt("FullScreenMode", (int)currentSettings.FullScreenMode);

        PlayerPrefs.SetInt("ResolutionX", currentSettings.Resolution.x);
        PlayerPrefs.SetInt("ResolutionY", currentSettings.Resolution.y);

        PlayerPrefs.SetFloat("SFXVolume", currentSettings.SFXVolume);
        PlayerPrefs.SetFloat("UIVolume", currentSettings.UIVolume);
        PlayerPrefs.SetFloat("MusicVolume", currentSettings.MusicVolume);
    }

    private void LoadSettings() {
        currentSettings.CameraShake = PlayerPrefs.GetFloat("CameraShake", currentSettings.CameraShake);

        currentSettings.vSync = PlayerPrefs.GetInt("vSync", currentSettings.vSync ? 1 : 0) == 1; // use 0 as false and 1 as true
        currentSettings.FullScreenMode = (FullScreenMode)PlayerPrefs.GetInt("FullScreenMode", (int)currentSettings.FullScreenMode);

        int resolutionX = PlayerPrefs.GetInt("ResolutionX", currentSettings.Resolution.x);
        int resolutionY = PlayerPrefs.GetInt("ResolutionY", currentSettings.Resolution.y);
        currentSettings.Resolution = new Vector2Int(resolutionX, resolutionY);

        currentSettings.SFXVolume = PlayerPrefs.GetFloat("SFXVolume", currentSettings.SFXVolume);
        currentSettings.UIVolume = PlayerPrefs.GetFloat("UIVolume", currentSettings.UIVolume);
        currentSettings.MusicVolume = PlayerPrefs.GetFloat("MusicVolume", currentSettings.MusicVolume);
    }
    #endregion
}




