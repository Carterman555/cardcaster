using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuInputManager : MonoBehaviour {

    [SerializeField] private Button playButton;

    private void OnEnable() {
        UpdateSelected();

        InputManager.OnControlsChanged += UpdateSelected;
    }

    private void OnDisable() {
        InputManager.OnControlsChanged -= UpdateSelected;
    }

    private void UpdateSelected() {
        if (InputManager.Instance.GetInputScheme() == ControlSchemeType.Controller) {
            playButton.Select();
        }

        if (InputManager.Instance.GetInputScheme() == ControlSchemeType.Keyboard) {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
