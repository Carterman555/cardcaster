using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopCard : MonoBehaviour {

    private Interactable interactable;
    private SpriteRenderer spriteRenderer;

    private ScriptableCardBase card;

    [SerializeField] private ChangeColorFromRarity changeShineColor;

    private void Awake() {
        interactable = GetComponent<Interactable>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable() {
        interactable.OnInteract += OpenAllCardsUI;

        //... default card for debugging
        SetCard(ResourceSystem.Instance.GetCard(CardType.Fire));
    }
    private void OnDisable() {
        interactable.OnInteract -= OpenAllCardsUI;
    }

    public void SetCard(ScriptableCardBase card) {
        this.card = card;
        spriteRenderer.sprite = card.GetSprite();

        changeShineColor.SetColor(card.GetRarity());
    }

    private void OpenAllCardsUI() {
        FeedbackPlayer.Play("OpenAllCardsPanel");
        ShopUIManager.Instance.Activate(card, this);
    }
}
