using DG.Tweening;
using System.Linq;
using UnityEngine;

public class ChestCard : CardDrop, IChestItem {

    private Chest chest;

    public void Setup(Chest chest, Vector2 position) {
        this.chest = chest;

        transform.position = chest.transform.position;
        transform.DOLocalMove(position, duration: 0.3f).SetEase(Ease.OutSine);

        transform.localScale = Vector2.zero;
        transform.DOScale(Vector2.one, duration: 0.3f).SetEase(Ease.OutSine).OnComplete(() => {
            interactable.enabled = true;
        });
    }

    protected override void OnInteract() {
        base.OnInteract();
        chest.StartCoroutine(chest.OnSelectCollectable());

        // hide other collectables in chest
        for (int collectableIndex = 0; collectableIndex < chest.ChestItems.Count; collectableIndex++) {
            if (!chest.ChestItems[collectableIndex].Equals(this)) {
                chest.ChestItems[collectableIndex].ReturnToChest(duration: 0.5f);
            }
        }

        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.GainChestCard);
    }

    public void ReturnToChest(float duration) {
        interactable.enabled = false;

        transform.DOLocalMove(new Vector2(0, 0.5f), duration).SetEase(Ease.InSine);
        transform.DOScale(Vector2.zero, duration).SetEase(Ease.InSine);
    }
}
