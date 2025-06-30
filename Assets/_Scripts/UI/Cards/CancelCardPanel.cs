using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CancelCardPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public static event Action OnSetToCancel;
    public static event Action OnSetToPlay;

    public void OnPointerEnter(PointerEventData eventData) {

        if (InputManager.Instance.GetControlScheme() != ControlSchemeType.Keyboard) {
            return;
        }

        OnSetToCancel?.Invoke();

        transform.DOKill();

        float hoverScale = 1.2f;
        transform.DOScale(hoverScale, duration: 0.2f);
    }

    public void OnPointerExit(PointerEventData eventData) {

        if (InputManager.Instance.GetControlScheme() != ControlSchemeType.Keyboard) {
            return;
        }

        OnSetToPlay?.Invoke();

        transform.DOKill();
        transform.DOScale(1f, duration: 0.2f);
    }
}
