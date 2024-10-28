using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "MassiveSwordCard", menuName = "Cards/Massive Sword Card")]
public class ScriptableMassiveSwordCard : ScriptableStatsModifierCard {

    [Header("Visual")]
    [SerializeField] private Sprite bigSwordSprite;
    [SerializeField] private Sprite normalSwordSprite;

    protected override void Play(Vector2 position) {
        base.Play(position);

        GrowSword();
    }

    public override void Stop() {
        base.Stop();

        ShrinkSword();
        RemoveEffects();
    }

    private void GrowSword() {

        SpriteRenderer swordVisual = ReferenceSystem.Instance.PlayerSwordVisual;
        swordVisual.sprite = bigSwordSprite;

        float transitionDuration = 0.5f;

        float sizeMult = GetPlayerStatsModifier().SwordSizePercent.PercentToMult();
        Vector2 bigSwordStartingSize = Vector2.one * (1f/sizeMult);

        // grow sword, then add effects
        swordVisual.transform.localScale = bigSwordStartingSize;
        swordVisual.transform.DOScale(1f, transitionDuration).OnComplete(() => {
            ApplyEffects();
        });
    }

    private void ShrinkSword() {
        SpriteRenderer swordVisual = ReferenceSystem.Instance.PlayerSwordVisual;

        float transitionDuration = 0.5f;

        float sizeMult = GetPlayerStatsModifier().SwordSizePercent.PercentToMult();
        Vector2 bigSwordEndingSize = Vector2.one * (1f / sizeMult);

        // shrink sword, then switch back to orignal sword
        swordVisual.transform.DOScale(bigSwordEndingSize, transitionDuration).OnComplete(() => {
            swordVisual.sprite = normalSwordSprite;
            swordVisual.transform.localScale = Vector3.one;
        });
    }

    #region Handle Ability Effects

    private List<GameObject> effectPrefabs = new List<GameObject>();
    private List<GameObject> effectInstances = new List<GameObject>();

    public override void AddEffect(GameObject effectPrefab) {
        base.AddEffect(effectPrefab);
        effectPrefabs.Add(effectPrefab);
    }

    private void ApplyEffects() {
        foreach (GameObject effectPrefab in effectPrefabs) {
            GameObject effect = effectPrefab.Spawn(PlayerMeleeAttack.Instance.transform);
            effectInstances.Add(effect);
        }
    }

    private void RemoveEffects() {
        foreach (GameObject effect in effectInstances) {
            effect.gameObject.ReturnToPool();
        }
        effectPrefabs.Clear();
        effectInstances.Clear();
    }

    #endregion
}
