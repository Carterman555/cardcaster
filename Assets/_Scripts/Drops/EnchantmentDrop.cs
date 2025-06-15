using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnchantmentDrop : MonoBehaviour {

    private SpriteRenderer spriteRenderer;
    private Interactable interactable;
    private SuckMovement suckMovement;

    [SerializeField] private Transform shine;

    private ScriptableEnchantment enchantment;

    [SerializeField] private bool useDebugEnchantment;
    [ConditionalHide("debugStat")][SerializeField] private ScriptableEnchantment enchantmentToDebug;

    public EnchantmentDrop[] EnchantmentDropsInGroup { get; set; } = new EnchantmentDrop[0];

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        interactable = GetComponent<Interactable>();
        suckMovement = GetComponent<SuckMovement>();

        if (useDebugEnchantment) {
            Setup(enchantmentToDebug, transform.position);
        }
    }

    private void OnEnable() {
        interactable.OnChangeCanInteract += SetShowInfo;
        interactable.OnInteract += OnInteract;
    }
    private void OnDisable() {
        interactable.OnChangeCanInteract -= SetShowInfo;
        interactable.OnInteract -= OnInteract;

        shine.DOKill();
        spriteRenderer.DOKill();
    }

    private void SetShowInfo(bool show) {
        if (show) {
            //ChestItemInfoUI.Instance.SetEnchantmentInfo(enchantment);
        }
        else {
            ChestItemInfoUI.Instance.RemoveInfo();
        }
    }

    public void Setup(ScriptableEnchantment enchantment, Vector2 position) {
        this.enchantment = enchantment;

        spriteRenderer.sprite = enchantment.Sprite;

        interactable.enabled = false;

        shine.localScale = Vector3.zero;
        shine.DOScale(1f, duration: 1.5f);

        spriteRenderer.Fade(0f);
        spriteRenderer.DOFade(1f, duration: 1.5f).OnComplete(() => {
            interactable.enabled = true;
        });

        transform.DOLocalMove(position, duration: 0.3f).SetEase(Ease.OutSine);
    }

    private void OnInteract() {

        foreach (EnchantmentDrop enchantmentDrop in EnchantmentDropsInGroup) {
            interactable.enabled = false;
            if (enchantmentDrop != this) {
                enchantmentDrop.transform.DOScale(Vector2.zero, duration: 0.2f).SetEase(Ease.InSine).OnComplete(() => {
                    enchantmentDrop.transform.DOKill();
                    enchantmentDrop.gameObject.ReturnToPool();
                });
            }
        }

        suckMovement.enabled = true;
        suckMovement.Setup(PlayerMovement.Instance.CenterTransform);

        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.GainItem);

        suckMovement.OnReachTarget += ShrinkAndApplyModifier;
    }

    private void ShrinkAndApplyModifier() {
        suckMovement.OnReachTarget -= ShrinkAndApplyModifier;

        transform.DOScale(Vector2.zero, duration: 0.2f).SetEase(Ease.InSine).OnComplete(() => {
            StatsManager.AddPlayerStatModifiers(enchantment.StatModifiers);

            transform.DOKill();
            gameObject.ReturnToPool();
        });
    }

}
