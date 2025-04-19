using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class CameraLookInfluence : MonoBehaviour {

    [SerializeField] private float mouseInfluence = 3f;
    private CinemachineFramingTransposer framingTransposer;

    [SerializeField] private float joystickInfluence = 3f;
    [SerializeField] private InputActionReference aimAction;

    private Vector2 desiredOffset = Vector2.zero;

    private bool frozenCameraLook;
    private Vector2 frozenOffset;

    void Awake() {
        CinemachineVirtualCamera virtualCamera = GetComponent<CinemachineVirtualCamera>();
        framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    private void OnEnable() {
        ScriptableAbilityCardBase.OnStartPositioning += TryFreezeCameraLook;
        ScriptableAbilityCardBase.OnStopPositioning += TryUnfreezeCameraLook;
    }

    private void OnDisable() {
        ScriptableAbilityCardBase.OnStartPositioning -= TryFreezeCameraLook;
        ScriptableAbilityCardBase.OnStopPositioning -= TryUnfreezeCameraLook;

        framingTransposer.m_TrackedObjectOffset = Vector3.zero;
    }

    void Update() {
        
        if (InputManager.Instance.GetControlScheme() == ControlSchemeType.Keyboard) {
            desiredOffset = GetMouseOffset();
        }
        else if (InputManager.Instance.GetControlScheme() == ControlSchemeType.Controller) {
            desiredOffset = GetJoystickOffset();
        }
        else {
            desiredOffset = Vector2.zero;
            Debug.LogWarning($"ControlSchemeType not found {InputManager.Instance.GetControlScheme()}!");
        }

        Vector3 offset = frozenCameraLook ? frozenOffset : desiredOffset;
        framingTransposer.m_TrackedObjectOffset = new Vector3(Mathf.Abs(offset.x), offset.y, 0f);
    }

    private void TryFreezeCameraLook() {
        if (InputManager.Instance.GetControlScheme() == ControlSchemeType.Controller) {
            frozenCameraLook = true;
            frozenOffset = desiredOffset;
        }
    }

    private void TryUnfreezeCameraLook() {
        frozenCameraLook = false;
    }

    private Vector2 GetMouseOffset() {
        Vector2 mousePos = Input.mousePosition;
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 normalizeMousePos = (mousePos - screenCenter).normalized;
        Vector2 offset = normalizeMousePos * mouseInfluence;

        return offset;
    }

    private Vector2 GetJoystickOffset() {
        Vector2 offset = PlayerMeleeAttack.Instance.GetAttackDirection() * joystickInfluence;

        return offset;
    }
}