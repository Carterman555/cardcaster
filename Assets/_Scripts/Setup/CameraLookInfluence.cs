using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class CameraLookInfluence : MonoBehaviour {

    [SerializeField] private float mouseInfluence = 3f;
    private CinemachineFramingTransposer framingTransposer;

    [SerializeField] private float joystickInfluence = 3f;
    [SerializeField] private InputActionReference aimAction;

    void Awake() {
        CinemachineVirtualCamera virtualCamera = GetComponent<CinemachineVirtualCamera>();
        framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    void Update() {
        
        Vector2 offset = Vector2.zero;
        if (InputManager.Instance.GetInputScheme() == ControlSchemeType.Keyboard) {
            offset = GetMouseOffset();
        }
        else if (InputManager.Instance.GetInputScheme() == ControlSchemeType.Controller) {
            offset = GetJoystickOffset();
        }
        else {
            Debug.LogWarning($"ControlSchemeType not found {InputManager.Instance.GetInputScheme()}!");
        }

        framingTransposer.m_TrackedObjectOffset = new Vector3(Mathf.Abs(offset.x), offset.y, 0f);
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