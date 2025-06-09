using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlankMemoryCardDrop : MonoBehaviour {

    public static event Action OnPickup;

    private ScriptableCardBase scriptableCard;

    private SpriteRenderer spriteRenderer;
    private SuckMovement suckMovement;

    [SerializeField] private CardType[] blankMemoryCardTypes;

    [SerializeField] private Transform shine;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        suckMovement = GetComponent<SuckMovement>();
    }

    private void OnEnable() {
        scriptableCard = ResourceSystem.Instance.GetCardInstance(blankMemoryCardTypes.RandomItem());

        spriteRenderer.sprite = scriptableCard.Sprite;

        shine.localScale = Vector3.zero;
        shine.DOScale(1f, duration: 1.5f);

        spriteRenderer.Fade(0f);
        spriteRenderer.DOFade(1f, duration: 1.5f);
    }

    private void OnTriggerEnter2D(Collider2D collision) {

        if (!GameLayers.AllPlayerLayerMask.ContainsLayer(collision.gameObject.layer)) {
            Debug.LogError($"Trying to pickup card, but collision is not player layer! (name: {collision.name}, layer: {collision.gameObject.layer})");
            return;
        }

        if (scriptableCard == null) {
            Debug.LogError("Trying to pickup card, but scriptable card is null!");
            return;
        }

        // suck to player
        suckMovement.enabled = true;
        suckMovement.Setup(PlayerMovement.Instance.CenterTransform);

        suckMovement.OnReachTarget += ShrinkAndGainCard;
    }

    private void ShrinkAndGainCard() {
        suckMovement.OnReachTarget -= ShrinkAndGainCard;

        transform.DOScale(Vector2.zero, duration: 0.2f).SetEase(Ease.InSine).OnComplete(() => {
            DeckManager.Instance.GainCard(scriptableCard);

            transform.DOKill();
            gameObject.ReturnToPool();

            OnPickup?.Invoke();
        });
    }
}
