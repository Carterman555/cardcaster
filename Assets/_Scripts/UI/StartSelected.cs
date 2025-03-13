using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartSelected : MonoBehaviour {

    private Selectable selectable;

    private void Awake() {
        selectable = GetComponent<Selectable>();
    }

    private void OnEnable() {
        TrySelect();

        InputManager.OnControlSchemeChanged += TrySelect;
    }

    private void OnDisable() {
        InputManager.OnControlSchemeChanged -= TrySelect;
    }

    private void TrySelect() {
        if (InputManager.Instance.GetControlScheme() == ControlSchemeType.Controller) {
            selectable.Select();
        }
    }
}
