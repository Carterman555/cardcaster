using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CancelCardText : MonoBehaviour {

    [SerializeField] private InputActionReference cancelCardAction;

    public string Input; // for localize string event (it sets the text)

    private void OnEnable() {
        Input = InputManager.Instance.GetBindingText(cancelCardAction);
    }
}
