using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItem : MonoBehaviour {

    private Interactable interactable;
    private SpriteRenderer spriteRenderer;

    private ScriptableCardBase card;

    [SerializeField] private ScriptableCardBase testCard;

    private void Awake() {
        interactable = GetComponent<Interactable>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        SetCard(testCard);
    }

    private void OnEnable() {
        interactable.OnInteract += OpenAllCardsUI;
    }
    private void OnDisable() {
        interactable.OnInteract -= OpenAllCardsUI;
    }

    public void SetCard(ScriptableCardBase card) {
        this.card = card;
        spriteRenderer.sprite = card.GetSprite();
    }

    private void OpenAllCardsUI() {
        FeedbackPlayer.Play("OpenAllCardsPanel");
        ShopUIManager.Instance.Activate(card, this);
    }
}
