using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : StaticInstance<InputManager> {

    [SerializeField] private PlayerInput playerInput;

    #region InputScheme

    public static event Action<ControlSchemeType> OnInputSchemeChanged;

    public ControlSchemeType GetInputScheme() {

        switch (playerInput.currentControlScheme) {
            case "Keyboard":
                return ControlSchemeType.Keyboard;
            case "Controller":
                return ControlSchemeType.Controller;
        }

        Debug.LogError("Could not find control scheme by string: " + playerInput.currentControlScheme);
        return default;
    }

    #endregion

    #region Update Action Map

    private ActionMapUpdaterPanel[] actionMapUpdaters;

    private void Start() {
        actionMapUpdaters = FindObjectsOfType<ActionMapUpdaterPanel>(true);
    }

    private void OnEnable() {
        ActionMapUpdaterPanel.OnAnyActiveChanged += UpdateActionMap;
    }

    private void OnDisable() {
        ActionMapUpdaterPanel.OnAnyActiveChanged -= UpdateActionMap;
    }

    private void UpdateActionMap() {
        bool anyActive = actionMapUpdaters.Any(u => u.isActiveAndEnabled);

        if (anyActive) {
            playerInput.SwitchCurrentActionMap("UI");
        }
        else {
            playerInput.SwitchCurrentActionMap("Gameplay");
        }
    }
    #endregion
}

public enum ControlSchemeType {
    Keyboard = 0,
    Controller = 1,
}