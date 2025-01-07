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

    public string GetBindingText(InputAction action) {

        string displayString;

        if (GetInputScheme() == ControlSchemeType.Keyboard) {
            displayString = action.bindings[0].ToDisplayString();
        }
        else if (GetInputScheme() == ControlSchemeType.Controller) {
            displayString = action.bindings[1].ToDisplayString();
        }
        else {
            Debug.LogError("Could not find input scheme: " + GetInputScheme());
            return null;
        }

        Dictionary<string, string> actionReplaceDict = new() {
            { "LMB", "left click" },
            { "RMB", "right click" },
        };

        if (actionReplaceDict.ContainsKey(displayString)) {
            return actionReplaceDict[displayString];
        }
        else {
            return displayString;
        }
    }
}

public enum ControlSchemeType {
    Keyboard = 0,
    Controller = 1,
}