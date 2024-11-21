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
    protected SuckMovement suckMovement;

    [SerializeField] private ChangeColorFromRarity changeShineColor;

    [SerializeField] private ScriptableCardBase defaultCard; // for testing

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        interactable = GetComponent<Interactable>();
        suckMovement = GetComponent<SuckMovement>();

        if (defaultCard != null) {
            Setup(defaultCard);
        }
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

        suckMovement.enabled = true;
        suckMovement.Setup(PlayerMovement.Instance.transform);

        suckMovement.OnReachTarget += ShrinkAndGainCard;
    }

    public void ShrinkAndGainCard() {
        suckMovement.OnReachTarget -= ShrinkAndGainCard;

        transform.DOScale(Vector2.zero, duration: 0.2f).SetEase(Ease.InSine).OnComplete(() => {
            DeckManager.Instance.GainCard(scriptableCard);

            transform.DOKill();
            gameObject.ReturnToPool();
        });
    }
}
