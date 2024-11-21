using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CardDrop : MonoBehaviour {
    [SerializeField] private InputActionReference selectAction;

    protected ScriptableCardBase scriptableCard;

    private SpriteRenderer spriteRenderer;
    protected Interactable interactable;

    [SerializeField] private ChangeColorFromRarity changeShineColor;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        interactable = GetComponent<Interactable>();
    }

    private void OnEnable() {
        interactable.OnInteract += OnInteract;
    }
    private void OnDisable() {
        interactable.OnInteract -= OnInteract;
    }

    public void Setup(ScriptableCardBase scriptableCard) {
        this.scriptableCard = scriptableCard;

        spriteRenderer.sprite = scriptableCard.GetSprite();

        interactable.enabled = false;

        float duration = 0.3f;
        transform.localScale = Vector2.zero;
        transform.DOScale(Vector2.one, duration).SetEase(Ease.OutSine).OnComplete(() => {
            interactable.enabled = true;
        });

        changeShineColor.SetColor(scriptableCard.GetRarity());
    }

    protected virtual void OnInteract() {
        GoToPlayer();
    }

    public void GoToPlayer() {

        interactable.enabled = false;

        //... so it doesn't disappear when the chest does
        transform.SetParent(Containers.Instance.Drops);

        float duration = 0.2f;
        transform.DOMove(PlayerMovement.Instance.transform.position, duration).SetEase(Ease.InSine).OnComplete(() => {
            transform.DOScale(Vector2.zero, duration).SetEase(Ease.InSine).OnComplete(() => {
                DeckManager.Instance.GainCard(scriptableCard);

                transform.DOKill();
                gameObject.ReturnToPool();
            });
        });
    }
}
