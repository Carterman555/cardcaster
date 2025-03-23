using DG.Tweening;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class ChestHeal : MonoBehaviour, IChestItem {

    public static readonly float HealAmount = 10f;

    private Interactable interactable;
    private SuckMovement suckMovement;

    private Chest chest;
    private int collectableIndex;

    private void Awake() {
        interactable = GetComponent<Interactable>();
        suckMovement = GetComponent<SuckMovement>();
    }

    private void OnEnable() {
        interactable.OnChangeCanInteract += SetShowHealInfo;
        interactable.OnInteract += OnInteract;
    }
    private void OnDisable() {
        interactable.OnChangeCanInteract -= SetShowHealInfo;
        interactable.OnInteract -= OnInteract;
    }

    public void Setup(Chest chest, int collectableIndex, Vector2 position) {
        this.chest = chest;
        this.collectableIndex = collectableIndex;

        transform.position = chest.transform.position;
        transform.DOLocalMove(position, duration: 0.3f).SetEase(Ease.OutSine);

        transform.localScale = Vector2.zero;
        transform.DOScale(Vector2.one, duration: 0.3f).SetEase(Ease.OutSine).OnComplete(() => {
            interactable.enabled = true;
        });
    }

    private void SetShowHealInfo(bool show) {
        if (show) {
            ChestItemInfoUI.Instance.SetHealInfo();
        }
        else {
            ChestItemInfoUI.Instance.RemoveInfo();
        }
    }

    private void OnInteract() {
        GoToPlayer();

        StartCoroutine(chest.OnSelectCollectable(collectableIndex));

        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.GainChestCard);
    }

    public void GoToPlayer() {

        interactable.enabled = false;

        //... so it doesn't disappear when the chest does
        transform.SetParent(Containers.Instance.Drops);

        suckMovement.enabled = true;
        suckMovement.Setup(PlayerMovement.Instance.transform);

        suckMovement.OnReachTarget += ShrinkAndHeal;
    }

    public void ShrinkAndHeal() {
        suckMovement.OnReachTarget -= ShrinkAndHeal;

        transform.DOScale(Vector2.zero, duration: 0.2f).SetEase(Ease.InSine).OnComplete(() => {
            PlayerMeleeAttack.Instance.GetComponent<PlayerHealth>().Heal(HealAmount);

            transform.DOKill();
            gameObject.ReturnToPool();
        });
    }

    public void ReturnToChest(float duration) {
        interactable.enabled = false;

        transform.DOLocalMove(new Vector2(0, 0.5f), duration).SetEase(Ease.InSine);
        transform.DOScale(Vector2.zero, duration).SetEase(Ease.InSine);
    }
}
