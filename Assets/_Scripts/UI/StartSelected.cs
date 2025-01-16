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

        InputManager.OnControlsChanged += TrySelect;
    }

    private void OnDisable() {
        InputManager.OnControlsChanged -= TrySelect;
    }

    private void TrySelect() {
        if (InputManager.Instance.GetInputScheme() == ControlSchemeType.Controller) {
            selectable.Select();
            print($"Select: {name}");
        }
    }
}
