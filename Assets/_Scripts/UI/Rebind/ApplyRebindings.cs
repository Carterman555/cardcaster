using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Samples.RebindUI;

public class ApplyRebindingsButton : GameButton {

    public static event Action OnApplyRebindingsClicked;

    [SerializeField] private Transform bindingUIContainer;
    private DuplicateBindingChecker[] duplicateBindingCheckers;

    private RebindActionUI firstRebindActionUI;

    [SerializeField] private InputActionAsset actions;

    [SerializeField] private GameObject backButton;

    protected override void OnEnable() {
        base.OnEnable();

        var rebinds = PlayerPrefs.GetString("rebinds");
        if (!string.IsNullOrEmpty(rebinds)) {
            actions.LoadBindingOverridesFromJson(rebinds);
        }

        duplicateBindingCheckers = bindingUIContainer.GetComponentsInChildren<DuplicateBindingChecker>();

        // only need to listen to one
        firstRebindActionUI = duplicateBindingCheckers[0].GetComponent<RebindActionUI>();
        firstRebindActionUI.updateBindingUIEvent.AddListener(OnUpdateUI);

        button.interactable = false;
    }

    protected override void OnDisable() {
        base.OnDisable();

        firstRebindActionUI.updateBindingUIEvent.RemoveListener(OnUpdateUI);
    }

    protected override void OnClick() {
        base.OnClick();

        OnApplyRebindingsClicked?.Invoke();

        // save after OnApplyRebindingsClicked event, so duplicateBindingCheckers can set the override bindings
        var rebinds = actions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("rebinds", rebinds);

        button.interactable = false;

        if (InputManager.Instance.GetControlScheme() == ControlSchemeType.Controller) {
            EventSystem.current.SetSelectedGameObject(backButton);
        }
    }

    private void OnUpdateUI(RebindActionUI rebindActionUI, string displayString, string deviceLayoutName, string controlPath) {
        StartCoroutine(OnUpdateUICor());
    }
    private IEnumerator OnUpdateUICor() {

        //... wait a frame to give all the duplicateBindingCheckers a chance to update
        //... their ContainsDuplicate
        yield return null;

        bool duplicateBindings = CheckDuplicateBindings();
        button.interactable = !duplicateBindings;
    }


    private bool CheckDuplicateBindings() {
        bool anyDuplicates = duplicateBindingCheckers.Any(d => d.ContainsDuplicate);
        return anyDuplicates;
    }
}
