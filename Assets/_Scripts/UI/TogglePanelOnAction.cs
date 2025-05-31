using MoreMountains.Feedbacks;
using QFSW.QC;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class TogglePanelOnAction : MonoBehaviour {

    [SerializeField] private InputActionReference actionReference;

    private MMF_Player feedbackPlayer;

    [SerializeField] private GameObject[] otherPanels;

    private void Awake() {
        feedbackPlayer = GetComponent<MMF_Player>();
    }

    private void OnEnable() {
        actionReference.action.performed += OnActionPerformed;
    }
    private void OnDisable() {
        actionReference.action.performed -= OnActionPerformed;
    }

    private void OnActionPerformed(InputAction.CallbackContext context) {

        if (QuantumConsole.Instance != null && QuantumConsole.Instance.IsActive) {
            return;
        }

        if (PauseManager.Instance.IsPaused()) {
            return;
        }

        if (otherPanels.Any(p => p.activeSelf)) {
            return;
        }

        feedbackPlayer.PlayFeedbacks();
    }
}
