using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButtonInteractVisual : GameButton, IPointerEnterHandler, IPointerExitHandler {

    [SerializeField] private Transform interactVisual;
    [SerializeField] private float visualWidth;

    public void OnPointerEnter(PointerEventData eventData) {
        interactVisual.DOScaleX(visualWidth, duration: 0.1f).SetUpdate(true).SetEase(Ease.InFlash);
    }

    public void OnPointerExit(PointerEventData eventData) {
        interactVisual.DOScaleX(0, duration: 0.1f).SetUpdate(true).SetEase(Ease.OutFlash);
    }
}
