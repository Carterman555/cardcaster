using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public abstract class ScriptableAbilityCardBase : ScriptableCardBase {

    [SerializeField] private AbilityAttribute abilityAttributes;
    public AbilityAttribute AbilityAttributes => abilityAttributes;

    [SerializeField] private AbilityStats abilityStats;
    public AbilityStats Stats => abilityStats;

    [field: SerializeField] public bool IsPositional { get; private set; }
    [field: SerializeField] public bool IsModifiable { get; private set; } = true;

    public bool IsCompatible(ScriptableModifierCardBase modifier) {
        return (abilityAttributes & modifier.AbilityAttributes) != 0;
    }

    private Coroutine draggingCardCoroutine;

    public virtual void OnStartDraggingCard(Transform cardTransform) {
        draggingCardCoroutine = AbilityManager.Instance.StartCoroutine(DraggingCard(cardTransform));
    }

    private IEnumerator DraggingCard(Transform cardTransform) {
        while (true) {
            yield return null;
            DraggingUpdate(Camera.main.ScreenToWorldPoint(cardTransform.position));
        }
    }

    protected virtual void DraggingUpdate(Vector2 cardposition) { }

    public override void Play(Vector2 position) {
        base.Play(position);

        if (draggingCardCoroutine != null) {
            AbilityManager.Instance.StopCoroutine(draggingCardCoroutine);
        }

        if (IsModifiable) {
            AbilityManager.Instance.ApplyModifiers(this);
            DeckManager.Instance.DiscardStackedCards();
        }

        AbilityManager.Instance.StartCoroutine(StopAfterDuration());
    }

    private IEnumerator StopAfterDuration() {
        yield return new WaitForSeconds(abilityStats.Duration);
        Stop();
    }

    public virtual void Stop() {
    }

    public virtual void AddEffect(GameObject effectPrefab) {
    }
}

[Flags]
public enum AbilityAttribute {
    None = 0,
    DealsDamage = 1 << 0,
    HasArea = 1 << 1,
    HasDuration = 1 << 2,
    IsProjectile = 1 << 3,
    HasCooldown = 1 << 4,
}

[Serializable]
public class AbilityStats {
    [ConditionalHideFlag("abilityAttributes", AbilityAttribute.DealsDamage)]
    public float Damage;

    [ConditionalHideFlag("abilityAttributes", AbilityAttribute.DealsDamage)]
    public float KnockbackStrength;

    [ConditionalHideFlag("abilityAttributes", AbilityAttribute.HasArea)]
    public float AreaSize;

    [ConditionalHideFlag("abilityAttributes", AbilityAttribute.HasDuration)]
    public float Duration;

    [ConditionalHideFlag("abilityAttributes", AbilityAttribute.IsProjectile)]
    public float ProjectileSpeed;

    [ConditionalHideFlag("abilityAttributes", AbilityAttribute.HasCooldown)]
    public float Cooldown;

    public void ApplyModifier(AbilityStats statsModifierPercentage, AbilityAttribute abilityAttributesToModify) {
        if (abilityAttributesToModify.HasFlag(AbilityAttribute.DealsDamage)) {
            Damage *= statsModifierPercentage.Damage.PercentToMult();
            KnockbackStrength *= statsModifierPercentage.KnockbackStrength.PercentToMult();
        }
        if (abilityAttributesToModify.HasFlag(AbilityAttribute.HasArea)) {
            AreaSize *= statsModifierPercentage.AreaSize.PercentToMult();
        }
        if (abilityAttributesToModify.HasFlag(AbilityAttribute.HasDuration)) {
            Duration *= statsModifierPercentage.Duration.PercentToMult();
        }
        if (abilityAttributesToModify.HasFlag(AbilityAttribute.IsProjectile)) {
            ProjectileSpeed *= statsModifierPercentage.ProjectileSpeed.PercentToMult();
        }
        if (abilityAttributesToModify.HasFlag(AbilityAttribute.HasCooldown)) {
            Cooldown *= statsModifierPercentage.Cooldown.PercentToMult();
        }
    }
}
