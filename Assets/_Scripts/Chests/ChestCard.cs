using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChestCard : CardDrop, IChestItem {

    private Chest chest;
    private int collectableIndex;

    public void Setup(Chest chest, int collectableIndex, Vector2 position) {
        this.chest = chest;
        this.collectableIndex = collectableIndex;

        transform.position = chest.transform.position;
        transform.DOLocalMove(position, duration: 0.3f).SetEase(Ease.OutSine);
    }

    protected override void OnInteract() {
        base.OnInteract();
        StartCoroutine(chest.OnSelectCollectable(collectableIndex));

        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.GainChestCard);
    }

    public void ReturnToChest(float duration) {
        interactable.enabled = false;

        transform.DOLocalMove(new Vector2(0, 0.5f), duration).SetEase(Ease.InSine);
        transform.DOScale(Vector2.zero, duration).SetEase(Ease.InSine);
    }
}
