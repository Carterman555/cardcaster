using DG.Tweening;
using UnityEngine;

public class CollapseItemArrow : GameButton {

    // on click, toggle whether the items are showing
    protected override void OnClicked() {
        base.OnClicked();

        string itemName = "Items";
        if (PopupCanvas.Instance.IsPopupActive(itemName)) {
            PopupCanvas.Instance.DeactivateUIObject(itemName, false);

            float duration = 0.3f;
            transform.DOScaleY(1, duration).SetEase(Ease.InSine);
        }
        else {
            PopupCanvas.Instance.ActivateUIObject(itemName);

            float duration = 0.3f;
            transform.DOScaleY(-1, duration).SetEase(Ease.InSine);
        }
    }
}
