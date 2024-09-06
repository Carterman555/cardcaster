using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateUIButton : GameButton {

    [SerializeField] private string objectToActivate;

    protected override void OnClicked() {
        base.OnClicked();
        PopupCanvas.Instance.ActivateUIObject(objectToActivate);
    }

}
