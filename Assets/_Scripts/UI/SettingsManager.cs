using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour {

    public static event Action OnSettingsChanged;

    [Header("General")]
    [SerializeField] private Slider cameraShakeSlider;

    [Serializable]
    public class GameSettings {
        public float CameraShake = 0.5f;
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
    }

    public void OnCameraShakerChanged(float value) {
        currentSettings.CameraShake = value;
        OnSettingsChanged?.Invoke();
    }

    public void ResetToDefaults() {
        currentSettings = new GameSettings();
        UpdateSliders();

        OnSettingsChanged?.Invoke();
    }
}
