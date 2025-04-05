using Cinemachine;
using UnityEngine;

public class CameraShaker : StaticInstance<CameraShaker> {

    [SerializeField] private CinemachineImpulseListener[] impulseListeners;

    [SerializeField] private float maxReactionGain = 4f;

    private CinemachineImpulseSource impulseSource;

    protected override void Awake() {
        base.Awake();
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void OnEnable() {
        SettingsManager.OnSettingsChanged += UpdateCameraShakeAmount;
        UpdateCameraShakeAmount();
    }

    private void OnDisable() {
        SettingsManager.OnSettingsChanged -= UpdateCameraShakeAmount;
    }

    private void UpdateCameraShakeAmount() {
        float shakeAmount = SettingsManager.GetSettings().CameraShake;
        foreach (var listener in impulseListeners) {
            listener.m_Gain = shakeAmount;
            listener.m_ReactionSettings.m_AmplitudeGain = Mathf.Lerp(0f, maxReactionGain, shakeAmount);
        }
    }

    public void ShakeCamera(float intensity, float duration = 0.2f) {
        impulseSource.m_ImpulseDefinition.m_ImpulseDuration = duration;
        impulseSource.GenerateImpulseWithForce(intensity);
    }
}
