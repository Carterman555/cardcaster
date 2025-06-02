using System;
using System.Collections;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour, IInitializable {

    public static event Action OnSettingsChanged;

    [Header("General")]
    [SerializeField] private Slider cameraShakeSlider;
    [SerializeField] private TMP_Dropdown languageDropdown;

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

    [Header("Accessibility")]
    [SerializeField] private Toggle autoAttackToggle;
    [SerializeField] private Toggle autoAimToggle;

    [Serializable]
    public class GameSettings {
        public float CameraShake = 0.5f;
        public string Language = "not set";

        public bool vSync = true;
        public FullScreenMode FullScreenMode = FullScreenMode.ExclusiveFullScreen;
        public Vector2Int Resolution = new Vector2Int(1920, 1080);

        public float SFXVolume = 0.5f;
        public float UIVolume = 0.5f;
        public float MusicVolume = 0.5f;

        public bool AutoAttack = false;
        public bool AutoAim = false;
    }

    public static GameSettings CurrentSettings;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void Init() {
        CurrentSettings = new();
    }

    public void Initialize() {
        LoadSettings();
        ScriptInitializer.Instance.StartCoroutine(SetMixerVolumes());
    }

    private IEnumerator SetMixerVolumes() {
        yield return null; // delay to wait for mixers to setup
        audioMixer.SetFloat("SfxVolume", AudioManager.SliderValueToDecibels(CurrentSettings.SFXVolume));
        audioMixer.SetFloat("UiVolume", AudioManager.SliderValueToDecibels(CurrentSettings.UIVolume));
        audioMixer.SetFloat("MusicVolume", AudioManager.SliderValueToDecibels(CurrentSettings.MusicVolume, maxDB: -6f));
    }

    private void OnEnable() {
        UpdateUI();
    }

    private void OnDisable() {
        SaveSettings();
    }

    private void Update() {
        bool screenResolutionChanged = CurrentSettings.Resolution.x != Screen.width || CurrentSettings.Resolution.y != Screen.height;
        if (screenResolutionChanged) {
            CurrentSettings.Resolution = new(Screen.width, Screen.height);
            currentResolutionText.text = $"{Screen.width} x {Screen.height}";

            UpdateResolutionDropdown();
        }
    }

    private void UpdateUI() {
        cameraShakeSlider.value = CurrentSettings.CameraShake;

        UpdateLanguageDropdown();

        vSyncToggle.isOn = CurrentSettings.vSync;

        SFXVolumeSlider.value = CurrentSettings.SFXVolume;
        UIVolumeSlider.value = CurrentSettings.UIVolume;
        musicVolumeSlider.value = CurrentSettings.MusicVolume;

        autoAttackToggle.isOn = CurrentSettings.AutoAttack;
        autoAimToggle.isOn = CurrentSettings.AutoAim;

        UpdateFullScreenModeDropdown();
        UpdateResolutionDropdown();
    }

    private void UpdateLanguageDropdown() {
        if (CurrentSettings.Language == "en") {
            languageDropdown.value = 0;
        }
        else if (CurrentSettings.Language == "zh-Hans") {
            languageDropdown.value = 1;
        }
    }

    private void UpdateFullScreenModeDropdown() {
        if (CurrentSettings.FullScreenMode == FullScreenMode.ExclusiveFullScreen) {
            displayModeDropDown.value = 0;
        }
        else if (CurrentSettings.FullScreenMode == FullScreenMode.FullScreenWindow) {
            displayModeDropDown.value = 1;
        }
        else if (CurrentSettings.FullScreenMode == FullScreenMode.Windowed) {
            displayModeDropDown.value = 2;
        }
    }

    private void UpdateResolutionDropdown() {
        if (resolutions.Contains(CurrentSettings.Resolution)) {
            int resolutionIndex = Array.IndexOf(resolutions, CurrentSettings.Resolution);
            resolutionDropDown.value = resolutionIndex;
        }
        else {
            //... make dropdown have "-"
            resolutionDropDown.value = resolutions.Length;
        }
    }

    #region On Settings Changed Methods

    public void OnCameraShakerChanged(float value) {
        CurrentSettings.CameraShake = value;
        OnSettingsChanged?.Invoke();
    }

    public void OnLanguageDropdownChanged(int languageValue) {
        
        if (languageValue == 0) {
            string englishCode = "en";
            CurrentSettings.Language = englishCode;
            LocalizationManager.Instance.SetUnityLanguage(englishCode);
        }
        else if (languageValue == 1) {
            string chineseCode = "zh-Hans";
            CurrentSettings.Language = chineseCode;
            LocalizationManager.Instance.SetUnityLanguage(chineseCode);
        }

        OnSettingsChanged?.Invoke();
    }

    public void OnVSyncToggled(bool active) {
        CurrentSettings.vSync = active;
        QualitySettings.vSyncCount = active ? 1 : 0;
        OnSettingsChanged?.Invoke();
    }

    public void OnScreenModeChanged(int screenModeValue) {

        if (screenModeValue == 0) {
            CurrentSettings.FullScreenMode = FullScreenMode.ExclusiveFullScreen;
        }
        else if (screenModeValue == 1) {
            CurrentSettings.FullScreenMode = FullScreenMode.FullScreenWindow;
        }
        else if (screenModeValue == 2) {
            CurrentSettings.FullScreenMode = FullScreenMode.Windowed;
        }

        Screen.fullScreenMode = CurrentSettings.FullScreenMode;

        OnSettingsChanged?.Invoke();
    }

    public void OnResolutionChanged(int dropdownValue) {

        bool valueOutsideRange = dropdownValue > resolutions.Length - 1;
        if (valueOutsideRange) {
            return;
        }

        Screen.SetResolution(resolutions[dropdownValue].x, resolutions[dropdownValue].y, CurrentSettings.FullScreenMode);

        OnSettingsChanged?.Invoke();
    }

    // dont set audioMixer of sfx yet because sfx should be quiet when ui is open, the sfx volume is set in audiomanager
    public void OnSFXVolumeSliderChanged(float value) {
        CurrentSettings.SFXVolume = value;
        OnSettingsChanged?.Invoke();
    }

    public void OnUIVolumeSliderChanged(float value) {
        CurrentSettings.UIVolume = value;
        audioMixer.SetFloat("UiVolume", AudioManager.SliderValueToDecibels(value));
        OnSettingsChanged?.Invoke();
    }

    public void OnMusicVolumeSliderChanged(float value) {
        CurrentSettings.MusicVolume = value;
        //... make max db -6 to make music quieter
        audioMixer.SetFloat("MusicVolume", AudioManager.SliderValueToDecibels(value, maxDB: -6f));
        OnSettingsChanged?.Invoke();
    }

    public void OnAutoAttackToggled(bool active) {
        CurrentSettings.AutoAttack = active;
        OnSettingsChanged?.Invoke();
    }

    public void OnAutoAimToggled(bool active) {
        CurrentSettings.AutoAim = active;
        OnSettingsChanged?.Invoke();
    }

    public void ResetToDefaults() {
        CurrentSettings = new GameSettings();

        QualitySettings.vSyncCount = CurrentSettings.vSync ? 1 : 0;
        Screen.SetResolution(CurrentSettings.Resolution.x, CurrentSettings.Resolution.y, CurrentSettings.FullScreenMode);

        audioMixer.SetFloat("UiVolume", AudioManager.SliderValueToDecibels(CurrentSettings.UIVolume));
        audioMixer.SetFloat("MusicVolume", AudioManager.SliderValueToDecibels(CurrentSettings.MusicVolume, maxDB: -6f));

        UpdateUI();

        OnSettingsChanged?.Invoke();
    }

    #endregion

    #region Save and Load Settings

    private void SaveSettings() {
        PlayerPrefs.SetFloat("CameraShake", CurrentSettings.CameraShake);
        PlayerPrefs.SetString("Language", CurrentSettings.Language);

        PlayerPrefs.SetInt("vSync", CurrentSettings.vSync ? 1 : 0); // use 0 as false and 1 as true
        PlayerPrefs.SetInt("FullScreenMode", (int)CurrentSettings.FullScreenMode);

        PlayerPrefs.SetInt("ResolutionX", CurrentSettings.Resolution.x);
        PlayerPrefs.SetInt("ResolutionY", CurrentSettings.Resolution.y);

        PlayerPrefs.SetFloat("SFXVolume", CurrentSettings.SFXVolume);
        PlayerPrefs.SetFloat("UIVolume", CurrentSettings.UIVolume);
        PlayerPrefs.SetFloat("MusicVolume", CurrentSettings.MusicVolume);

        PlayerPrefs.SetInt("autoAttack", CurrentSettings.AutoAttack ? 1 : 0); // use 0 as false and 1 as true
        PlayerPrefs.SetInt("autoAim", CurrentSettings.AutoAim ? 1 : 0); // use 0 as false and 1 as true
    }

    private void LoadSettings() {
        CurrentSettings.CameraShake = PlayerPrefs.GetFloat("CameraShake", CurrentSettings.CameraShake);

        ScriptInitializer.Instance.StartCoroutine(LoadLanguage());

        CurrentSettings.vSync = PlayerPrefs.GetInt("vSync", CurrentSettings.vSync ? 1 : 0) == 1; // use 0 as false and 1 as true
        CurrentSettings.FullScreenMode = (FullScreenMode)PlayerPrefs.GetInt("FullScreenMode", (int)CurrentSettings.FullScreenMode);

        int resolutionX = PlayerPrefs.GetInt("ResolutionX", CurrentSettings.Resolution.x);
        int resolutionY = PlayerPrefs.GetInt("ResolutionY", CurrentSettings.Resolution.y);
        CurrentSettings.Resolution = new Vector2Int(resolutionX, resolutionY);

        CurrentSettings.SFXVolume = PlayerPrefs.GetFloat("SFXVolume", CurrentSettings.SFXVolume);
        CurrentSettings.UIVolume = PlayerPrefs.GetFloat("UIVolume", CurrentSettings.UIVolume);
        CurrentSettings.MusicVolume = PlayerPrefs.GetFloat("MusicVolume", CurrentSettings.MusicVolume);

        CurrentSettings.AutoAttack = PlayerPrefs.GetInt("autoAttack", CurrentSettings.AutoAttack ? 1 : 0) == 1; // use 0 as false and 1 as true
        CurrentSettings.AutoAim = PlayerPrefs.GetInt("autoAim", CurrentSettings.AutoAim ? 1 : 0) == 1; // use 0 as false and 1 as true
    }

    private IEnumerator LoadLanguage() {

        // wait for steamManager to initialize
        yield return null;

        string defaultLanguage = LocalizationManager.GetLanguageCode();
        CurrentSettings.Language = PlayerPrefs.GetString("Language", defaultLanguage);
    }

    #endregion
}




