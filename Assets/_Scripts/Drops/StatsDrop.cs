using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsDrop : MonoBehaviour {

    private SpriteRenderer spriteRenderer;
    private Interactable interactable;
    private SuckMovement suckMovement;

    [SerializeField] private Transform shine;

    private ScriptableStatModifier statModifier;

    [SerializeField] private bool debugStat;
    [ConditionalHide("debugStat")][SerializeField] private ScriptableStatModifier statToDebug;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        interactable = GetComponent<Interactable>();
        suckMovement = GetComponent<SuckMovement>();

        if (debugStat) {
            Setup(statToDebug);
        }
    }

    private void OnEnable() {
        interactable.OnChangeCanInteract += SetShowStatInfo;
        interactable.OnInteract += GoToPlayer;
    }
    private void OnDisable() {
        interactable.OnChangeCanInteract -= SetShowStatInfo;
        interactable.OnInteract -= GoToPlayer;

        shine.DOKill();
        spriteRenderer.DOKill();
    }

    private void SetShowStatInfo(bool show) {
        if (show) {
            //ChestItemInfoUI.Instance.SetStatInfo(statModifier);
        }
        else {
            ChestItemInfoUI.Instance.RemoveInfo();
        }
    }

    private void Setup(ScriptableStatModifier statModifier) {
        this.statModifier = statModifier;

        spriteRenderer.sprite = statModifier.Sprite;

        interactable.enabled = false;

        // spawn visual
        shine.localScale = Vector3.zero;
        shine.DOScale(1f, duration: 1.5f);

        spriteRenderer.Fade(0f);
        spriteRenderer.DOFade(1f, duration: 1.5f).OnComplete(() => {
            interactable.enabled = true;
        });
    }

    private void GoToPlayer() {

        interactable.enabled = false;

        //... so it doesn't disappear when the chest does
        transform.SetParent(Containers.Instance.Drops);

        suckMovement.enabled = true;
        suckMovement.Setup(PlayerMovement.Instance.CenterTransform);

        suckMovement.OnReachTarget += ShrinkAndApplyModifier;
    }

    private void ShrinkAndApplyModifier() {
        suckMovement.OnReachTarget -= ShrinkAndApplyModifier;

        transform.DOScale(Vector2.zero, duration: 0.2f).SetEase(Ease.InSine).OnComplete(() => {
            StatsManager.AddPlayerStatModifiers(statModifier.StatModifiers);

            transform.DOKill();
            gameObject.ReturnToPool();
        });
    }

}
