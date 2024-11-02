using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItem : MonoBehaviour {

    private Interactable interactable;

    private void Awake() {
        interactable = GetComponent<Interactable>();
    }

    private void OnEnable() {
        interactable.OnInteract += OpenAllCardsUI;
    }
    private void OnDisable() {
        interactable.OnInteract -= OpenAllCardsUI;
    }

    private void OpenAllCardsUI() {
        FeedbackPlayer.Play("OpenAllCardsPanel");
        ShopUIManager.Instance.Activate();
    }
}
