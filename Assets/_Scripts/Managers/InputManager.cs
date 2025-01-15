using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEditor.PlayerSettings;

public class InputManager : Singleton<InputManager> {

    [SerializeField] private PlayerInput playerInput;

    private void OnEnable() {
        ActionMapUpdaterPanel.OnAnyActiveChanged += UpdateActionMap;
        SceneManager.sceneLoaded += UpdateGlobalActionMap;

        playerInput.controlsChangedEvent.AddListener(InvokeInputSchemeChangedEvent);
    }

    private void OnDisable() {
        ActionMapUpdaterPanel.OnAnyActiveChanged -= UpdateActionMap;
        SceneManager.sceneLoaded -= UpdateGlobalActionMap;

        playerInput.controlsChangedEvent.RemoveListener(InvokeInputSchemeChangedEvent);
    }

    #region InputScheme

    public static event Action OnControlsChanged;

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

    private void InvokeInputSchemeChangedEvent(PlayerInput playerInput) {
        OnControlsChanged?.Invoke();
    }

    #endregion

    #region Update Action Maps

    private ActionMapUpdaterPanel[] actionMapUpdaters;

    private void Start() {
        actionMapUpdaters = FindObjectsOfType<ActionMapUpdaterPanel>(true);
    }

    // there are different controls when UI is open
    private void UpdateActionMap() {
        bool anyActive = actionMapUpdaters.Any(u => u.isActiveAndEnabled);

        if (anyActive) {
            playerInput.SwitchCurrentActionMap("UI");
        }
        else {
            playerInput.SwitchCurrentActionMap("Gameplay");
        }
    }

    // I want the toggle map action (and maybe other actions) to be active whether UI is active or not, but not when in menu scene
    private void UpdateGlobalActionMap(Scene scene, LoadSceneMode loadSceneMode) {
        if (scene.name == "Game") {
            playerInput.actions.FindActionMap("GameGlobal").Enable();
        }
        else if (scene.name == "Menu") {
            playerInput.actions.FindActionMap("GameGlobal").Disable();
        }
    }

    #endregion

    #region Get Binding Text and Images

    public string GetBindingText(InputAction action, bool shortDisplayName = true) {

        if (action == null) {
            Debug.LogError("Action is null!");
        }

        string displayString;

        if (GetInputScheme() == ControlSchemeType.Keyboard) {

            //... get the binding of the active control scheme
            var binding = action.bindings
                .FirstOrDefault(b => b.groups.Contains(playerInput.currentControlScheme));

            if (shortDisplayName) {
                displayString = binding.ToDisplayString();
            }
            else {
                displayString = binding.ToDisplayString(InputBinding.DisplayStringOptions.DontUseShortDisplayNames);
            }
        }
        else if (GetInputScheme() == ControlSchemeType.Controller) {
            displayString = GetActionSpriteTag(action);
        }
        else {
            Debug.LogError("Could not find input scheme: " + GetInputScheme());
            return null;
        }

        Dictionary<string, string> actionReplaceDict = new() {
            { "exm", "example" },
        };

        if (actionReplaceDict.ContainsKey(displayString)) {
            return actionReplaceDict[displayString];
        }
        else {
            return displayString;
        }
    }

    [SerializeField] private GamepadIcons xboxIcons;
    [SerializeField] private GamepadIcons ps4Icons;

    public Sprite GetBindingImage(InputAction action) {
        Sprite icon = default;

        if (InputSystem.IsFirstLayoutBasedOnSecond(action.activeControl.layout, "DualShockGamepad"))
            icon = ps4Icons.GetSprite(action.activeControl.path);
        else if (InputSystem.IsFirstLayoutBasedOnSecond(action.activeControl.layout, "Gamepad"))
            icon = xboxIcons.GetSprite(action.activeControl.path);

        return icon;
    }

    [Serializable]
    public struct GamepadIcons {
        public Sprite buttonSouth;
        public Sprite buttonNorth;
        public Sprite buttonEast;
        public Sprite buttonWest;
        public Sprite startButton;
        public Sprite selectButton;
        public Sprite leftTrigger;
        public Sprite rightTrigger;
        public Sprite leftShoulder;
        public Sprite rightShoulder;
        public Sprite dpad;
        public Sprite dpadUp;
        public Sprite dpadDown;
        public Sprite dpadLeft;
        public Sprite dpadRight;
        public Sprite leftStick;
        public Sprite rightStick;
        public Sprite leftStickPress;
        public Sprite rightStickPress;

        public Sprite GetSprite(string controlPath) {
            // From the input system, we get the path of the control on device. So we can just
            // map from that to the sprites we have for gamepads.
            switch (controlPath) {
                case "buttonSouth": return buttonSouth;
                case "buttonNorth": return buttonNorth;
                case "buttonEast": return buttonEast;
                case "buttonWest": return buttonWest;
                case "start": return startButton;
                case "select": return selectButton;
                case "leftTrigger": return leftTrigger;
                case "rightTrigger": return rightTrigger;
                case "leftShoulder": return leftShoulder;
                case "rightShoulder": return rightShoulder;
                case "dpad": return dpad;
                case "dpad/up": return dpadUp;
                case "dpad/down": return dpadDown;
                case "dpad/left": return dpadLeft;
                case "dpad/right": return dpadRight;
                case "leftStick": return leftStick;
                case "rightStick": return rightStick;
                case "leftStickPress": return leftStickPress;
                case "rightStickPress": return rightStickPress;
            }
            return null;
        }
    }

    public string GetActionSpriteTag(InputAction action) {

        //... get the binding of the active control scheme
        var binding = action.bindings
            .FirstOrDefault(b => b.groups.Contains(playerInput.currentControlScheme));

        string actionDisplayStr = binding.ToDisplayString(InputBinding.DisplayStringOptions.DontOmitDevice);
        return $"<sprite name=\"{actionDisplayStr}\">";
    }

    #endregion
}

public enum ControlSchemeType {
    Keyboard = 0,
    Controller = 1,
}