using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CancelCardText : MonoBehaviour {

    [SerializeField] private InputActionReference cancelCardAction;

    private TextMeshProUGUI text;

    private void Awake() {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable() {
        text.text = $"{InputManager.Instance.GetBindingText(cancelCardAction)} to cancel card";
    }
}
