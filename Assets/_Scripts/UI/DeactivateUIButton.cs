using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateUIButton : GameButton {
    [SerializeField] private string objectToDeactivate;
    [SerializeField] private bool deactivate;

    protected override void OnClicked() {
        base.OnClicked();
        PopupCanvas.Instance.DeactivateUIObject(objectToDeactivate, deactivate);
    }
}
