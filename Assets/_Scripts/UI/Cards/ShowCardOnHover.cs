using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShowCardOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    private ShowCardMovement showCardMovement;

    private void Awake() {
        showCardMovement = GetComponent<ShowCardMovement>();
    }

    private void OnEnable() {
        // so can disable script
    }

    public void OnPointerEnter(PointerEventData eventData) {

        if (!enabled) return;

        showCardMovement.Show();
    }

    public void OnPointerExit(PointerEventData eventData) {

        if (!enabled) return;

        showCardMovement.Hide();
    }
}
