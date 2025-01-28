using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using static UnityEngine.InputManagerEntry;

public class SettingsManager : MonoBehaviour, IInitializable {

    public static event Action OnSettingsChanged;

    [Header("General")]
    [SerializeField] private Slider cameraShakeSlider;

    [Header("Video")]
    [SerializeField] private Toggle vSyncToggle;
    [SerializeField] private TMP_Dropdown displayModeDropDown;

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
        public DisplayMode displayMode = default;

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
        AudioManager.Instance.StartCoroutine(SetMixerVolumes());
    }

    private IEnumerator SetMixerVolumes() {
        yield return null; // delay to wait for mixers to setup
        audioMixer.SetFloat("SfxVolume", AudioManager.SliderValueToDecibels(currentSettings.SFXVolume));
        audioMixer.SetFloat("UiVolume", AudioManager.SliderValueToDecibels(currentSettings.UIVolume));
        audioMixer.SetFloat("MusicVolume", AudioManager.SliderValueToDecibels(currentSettings.MusicVolume, maxDB: -6f));
    }

    private void OnEnable() {
        UpdateSliders();
    }

    private void UpdateSliders() {
        cameraShakeSlider.value = currentSettings.CameraShake;
        SFXVolumeSlider.value = currentSettings.SFXVolume;
        UIVolumeSlider.value = currentSettings.UIVolume;
        musicVolumeSlider.value = currentSettings.MusicVolume;
    }

    public void OnCameraShakerChanged(float value) {
        currentSettings.CameraShake = value;
        OnSettingsChanged?.Invoke();
    }

    public void OnVSyncToggled(bool active) {
        QualitySettings.vSyncCount = active ? 1 : 0;
        OnSettingsChanged?.Invoke();
    }

    public void OnScreenModeChanged(int screenModeInt) {
        currentSettings.displayMode = (DisplayMode) screenModeInt;

        if (currentSettings.displayMode == DisplayMode.FullScreen) {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        }
        else if (currentSettings.displayMode == DisplayMode.Windowed) {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }

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
        UpdateSliders();

        OnSettingsChanged?.Invoke();
    }

    public enum DisplayMode { FullScreen, Windowed }
    
}
