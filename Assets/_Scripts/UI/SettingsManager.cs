using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using static UnityEngine.InputManagerEntry;

public class SettingsManager : MonoBehaviour {

    public static event Action OnSettingsChanged;

    [Header("General")]
    [SerializeField] private Slider cameraShakeSlider;

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

    public void OnSFXVolumeSliderChanged(float value) {
        currentSettings.SFXVolume = value;
        audioMixer.SetFloat("SfxVolume", SliderValueToDecibels(value));
        OnSettingsChanged?.Invoke();
    }

    public void OnUIVolumeSliderChanged(float value) {
        currentSettings.UIVolume = value;
        audioMixer.SetFloat("UiVolume", SliderValueToDecibels(value));
        OnSettingsChanged?.Invoke();
    }

    public void OnMusicVolumeSliderChanged(float value) {
        currentSettings.MusicVolume = value;
        audioMixer.SetFloat("MusicVolume", SliderValueToDecibels(value));
        print($"Set music to {SliderValueToDecibels(value)}");
        OnSettingsChanged?.Invoke();
    }

    public void ResetToDefaults() {
        currentSettings = new GameSettings();
        UpdateSliders();

        OnSettingsChanged?.Invoke();
    }

    #region Audio Conversion

    // changing the decibels in the audio mixers is nonlinear with how loud the volume is

    private float SliderValueToDecibels(float sliderValue) {

        //... the logarithmic conversion will not work correctly at zero
        sliderValue = Mathf.Max(0.0001f, sliderValue);

        float dB = Mathf.Log10(sliderValue) * 20;
        return dB;
    }

    #endregion
}
