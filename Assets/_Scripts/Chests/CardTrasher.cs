using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardTrasher : MonoBehaviour {

    [SerializeField] private TriggerContactTracker playerTracker;

    private bool used;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite putOutFire;
    private Sprite originalSprite;

    private void Awake() {
        originalSprite = spriteRenderer.sprite;
    }

    private void OnEnable() {
        playerTracker.OnEnterContact += TryOpenTrashUI;

        used = false;
        spriteRenderer.sprite = originalSprite;
    }
    private void OnDisable() {
        playerTracker.OnEnterContact -= TryOpenTrashUI;
    }

    private void TryOpenTrashUI(GameObject player) {

        if (used) {
            return;
        }

        FeedbackPlayer.Play("OpenAllCardsPanel");
        AllCardsPanel.Instance.SetCardToTrash(true);

        used = true;

        spriteRenderer.sprite = putOutFire;
    }
}
