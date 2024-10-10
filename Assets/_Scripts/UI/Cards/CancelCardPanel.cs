using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CancelCardPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public static event Action OnSetToCancel;
    public static event Action OnSetToPlay;

    public void OnPointerEnter(PointerEventData eventData) {
        OnSetToCancel?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData) {
        OnSetToPlay?.Invoke();
    }
}
