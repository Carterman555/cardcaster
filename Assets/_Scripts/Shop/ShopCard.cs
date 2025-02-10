using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopCard : MonoBehaviour {

    private Interactable interactable;
    private SpriteRenderer spriteRenderer;

    private ScriptableCardBase card;

    [SerializeField] private ChangeColorFromRarity changeShineColor;

    [SerializeField] private bool debugCard;
    [ConditionalHide("debugCard")] [SerializeField] private ScriptableCardBase defaultCard;

    private void Awake() {
        interactable = GetComponent<Interactable>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable() {
        interactable.OnInteract += OpenAllCardsUI;

        if (debugCard) {
            SetCard(defaultCard);
        }
    }
    private void OnDisable() {
        interactable.OnInteract -= OpenAllCardsUI;
    }

    public void SetCard(ScriptableCardBase card) {
        this.card = card;

        spriteRenderer.sprite = card.GetSprite();
        print($"{card.name}: set sprite: {card.GetSprite()}");

        changeShineColor.SetColor(card.GetRarity());
    }

    private void OpenAllCardsUI() {
        FeedbackPlayerReference.Play("OpenAllCardsPanel");
        TradeUIManager.Instance.Activate(card, this);
    }
}
