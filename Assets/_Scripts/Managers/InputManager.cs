using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : StaticInstance<InputManager> {


    #region Update Action Map
    [SerializeField] private PlayerInput playerInput;

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
