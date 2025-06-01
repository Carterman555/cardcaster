using UnityEngine;
using UnityEngine.InputSystem.Samples.RebindUI;

public class ResetAllBindingsButton : GameButton {

    [SerializeField] private Transform bindingUIContainer;
    private RebindActionUI[] rebindActionUIs;

    protected override void OnEnable() {
        base.OnEnable();

        rebindActionUIs = bindingUIContainer.GetComponentsInChildren<RebindActionUI>();
    }

    protected override void OnClick() {
        base.OnClick();

        foreach (RebindActionUI rebindActionUI in rebindActionUIs) {
            rebindActionUI.ResetToDefault();
        }
    }
}
