using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButtonInteractVisual : GameButton, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler {

    [SerializeField] private Transform interactVisual;
    [SerializeField] private float visualWidth;

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
        interactVisual.DOScaleX(visualWidth, duration: 0.1f).SetUpdate(true).SetEase(Ease.InFlash);
    }

    private void HideUnderline() {
        interactVisual.DOScaleX(0, duration: 0.1f).SetUpdate(true).SetEase(Ease.OutFlash);
    }

    
}
