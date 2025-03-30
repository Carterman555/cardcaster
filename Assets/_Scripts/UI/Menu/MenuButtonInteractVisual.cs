using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButtonInteractVisual : GameButton, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler {

    [SerializeField] private Transform interactVisual;

    [SerializeField] private TextMeshProUGUI text;

    protected override void OnEnable() {
        base.OnEnable();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        ShowUnderline();
    }
    public void OnPointerExit(PointerEventData eventData) {
        HideUnderline();
    }

    public void OnSelect(BaseEventData eventData) {
        ShowUnderline();
    }

    public void OnDeselect(BaseEventData eventData) {
        HideUnderline();
    }

    private void ShowUnderline() {
        TMP_TextInfo textInfo = text.textInfo;

        // Force an update to ensure text metrics are current
        text.ForceMeshUpdate();

        if (textInfo.characterCount > 0) {
            float minX = textInfo.characterInfo[0].bottomLeft.x;
            float maxX = textInfo.characterInfo[textInfo.characterCount - 1].topRight.x;
            float length = maxX - minX;
            interactVisual.DOScaleX(length, duration: 0.1f).SetUpdate(true).SetEase(Ease.InFlash);
        }
    }

    private void HideUnderline() {
        interactVisual.DOScaleX(0, duration: 0.1f).SetUpdate(true).SetEase(Ease.OutFlash);
    }
}
