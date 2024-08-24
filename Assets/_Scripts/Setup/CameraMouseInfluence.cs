using UnityEngine;
using Cinemachine;

public class CameraMouseInfluence : MonoBehaviour {

    [SerializeField] private float mouseInfluence = 0.1f;
    private CinemachineFramingTransposer framingTransposer;

    void Awake() {
        CinemachineVirtualCamera virtualCamera = GetComponent<CinemachineVirtualCamera>();
        framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    void Update() {
        Vector2 mousePos = Input.mousePosition;
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 normalizeMousePos = (mousePos - screenCenter).normalized;
        Vector2 offset = normalizeMousePos * mouseInfluence;

        framingTransposer.m_TrackedObjectOffset = new Vector3(Mathf.Abs(offset.x), offset.y, 0f);
    }
}