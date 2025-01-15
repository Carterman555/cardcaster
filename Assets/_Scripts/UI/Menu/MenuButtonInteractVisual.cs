using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButtonInteractVisual : GameButton, IPointerEnterHandler, IPointerExitHandler {

    [SerializeField] private Transform interactVisual;
    [SerializeField] private float visualWidth;

    protected override void OnEnable() {
        base.OnEnable();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        interactVisual.DOScaleX(visualWidth, duration: 0.1f).SetUpdate(true).SetEase(Ease.InFlash);
    }

    public void OnPointerExit(PointerEventData eventData) {
        interactVisual.DOScaleX(0, duration: 0.1f).SetUpdate(true).SetEase(Ease.OutFlash);
    }

    private void OnSelected() {
        interactVisual.DOScaleX(visualWidth, duration: 0.1f).SetUpdate(true).SetEase(Ease.InFlash);
    }
}
