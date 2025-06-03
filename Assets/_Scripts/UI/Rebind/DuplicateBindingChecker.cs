using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Samples.RebindUI;
using UnityEngine.UI;

public class DuplicateBindingChecker : MonoBehaviour {

    private RebindActionUI rebindActionUI;

    private int bindingIndex;
    private InputBinding appliedBinding;

    public bool ContainsDuplicate { get; private set; }

    [SerializeField] private Color duplicateColor;
    [SerializeField] private Image bindingButtonImage;

    private void Awake() {
        rebindActionUI = GetComponent<RebindActionUI>();
        
    }

    private void OnEnable() {
        rebindActionUI.updateBindingUIEvent.AddListener(OnUpdateBinding);
        ApplyRebindingsButton.OnApplyRebindingsClicked += ApplyBinding;

        bindingIndex = GetBindingIndex();
        appliedBinding = rebindActionUI.actionReference.action.bindings[bindingIndex];

        UpdateChecker();
    }

    private void OnDisable() {
        rebindActionUI.updateBindingUIEvent.RemoveListener(OnUpdateBinding);

        //... RebindActionUI applies an binding that could be a duplicate, so make sure to apply the binding
        //... that was chosen when the apply button was clicked
        rebindActionUI.actionReference.action.ApplyBindingOverride(bindingIndex, appliedBinding);
    }

    private void ApplyBinding() {
        InputBinding binding = rebindActionUI.actionReference.action.bindings[bindingIndex];

        if (CheckDuplicateBindings(binding)) {
            Debug.LogError($"{rebindActionUI.actionLabel.text}: Trying to apply duplicate binding! This should never happen!");
            return;
        }

        appliedBinding = binding;

        //... probably not needed because appliedBinding is applied OnDisable, but it doesn't hurt and ensures
        //... it's set
        rebindActionUI.actionReference.action.ApplyBindingOverride(appliedBinding);
    }

    private void OnUpdateBinding(RebindActionUI rebindActionUI, string displayString, string deviceLayoutName, string controlPath) {
        UpdateChecker();
    }

    private void UpdateChecker() {
        InputBinding binding = rebindActionUI.actionReference.action.bindings[bindingIndex];
        ContainsDuplicate = CheckDuplicateBindings(binding);

        bindingButtonImage.color = ContainsDuplicate ? duplicateColor : Color.white;
    }

    private int GetBindingIndex() {
        InputAction action = rebindActionUI.actionReference.action;
        string bindingIdStr = rebindActionUI.bindingId;

        if (action == null) {
            Debug.LogError($"{rebindActionUI.actionLabel.text}: Action is null!");
            return -1;
        }

        if (string.IsNullOrEmpty(bindingIdStr)) {
            Debug.LogError($"{rebindActionUI.actionLabel.text}: Binding ID is null!");
            return -1;
        }

        // Look up binding index.
        var bindingId = new Guid(bindingIdStr);
        int bindingIndex = action.bindings.IndexOf(x => x.id == bindingId);
        if (bindingIndex == -1) {
            Debug.LogError($"{rebindActionUI.actionLabel.text}: Cannot find binding with ID '{bindingId}' on '{action}'", this);
            return -1;
        }

        return bindingIndex;
    }

    private bool CheckDuplicateBindings(InputBinding newBinding) {
        PlayerInput playerInput = FindAnyObjectByType<PlayerInput>();
        InputActionMap gameplayActionMap = playerInput.actions.FindActionMap("Gameplay");
        InputActionMap gameGlobalActionMap = playerInput.actions.FindActionMap("GameGlobal");

        InputBinding[] allGameBindings = gameplayActionMap.bindings.Concat(gameGlobalActionMap.bindings).ToArray();

        foreach (InputBinding binding in allGameBindings) {

            // bindings can be the same as the next dialog input
            if (binding.action == "NextDialog") {
                continue;
            }

            if (newBinding.action == binding.action) {
                continue;
            }

            if (newBinding.effectivePath == binding.effectivePath) {
                return true;
            }
        }

        return false;
    }
}
