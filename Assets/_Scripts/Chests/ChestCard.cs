using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChestCard : CardDrop {

    [SerializeField] private Vector2 positionOffset;

    private Chest chest;
    private int collectableIndex;

    public void Setup(Chest chest, ScriptableCardBase scriptableCard, int collectableIndex) {
        this.chest = chest;
        this.collectableIndex = collectableIndex;
        Setup(scriptableCard);

        transform.position = chest.transform.position;
        transform.DOLocalMove(positionOffset, duration: 0.3f).SetEase(Ease.OutSine);
    }

    protected override void OnInteract() {
        base.OnInteract();
        StartCoroutine(chest.OnSelectCollectable(collectableIndex));
    }

    public void ReturnToChest(float duration) {
        interactable.enabled = false;

        transform.DOLocalMove(new Vector2(0, 0.5f), duration).SetEase(Ease.InSine);
        transform.DOScale(Vector2.zero, duration).SetEase(Ease.InSine);
    }
}
