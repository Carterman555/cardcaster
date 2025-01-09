using Mono.CSharp;
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

    private Coroutine positioningCardCoroutine;

    public virtual void OnStartPositioningCard(Transform cardTransform) {
        positioningCardCoroutine = AbilityManager.Instance.StartCoroutine(PositioningCard(cardTransform));
    }

    private IEnumerator PositioningCard(Transform cardTransform) {
        while (true) {
            yield return null;
            PositioningUpdate(Camera.main.ScreenToWorldPoint(cardTransform.position));
        }
    }

    protected virtual void PositioningUpdate(Vector2 cardposition) { }

    public virtual void OnStopPositioningCard() {
        AbilityManager.Instance.StopCoroutine(positioningCardCoroutine);
    }

    public override void TryPlay(Vector2 position) {
        base.TryPlay(position);

        // if multiple can't play at the same time, reset the duration of the current one playing
        if (CanStackWithSelf || !AbilityManager.Instance.IsAbilityActive(this, out ScriptableAbilityCardBase alreadyActiveAbility)) {
            Play(position);
        }
        else {
            alreadyActiveAbility.ResetDuration();
        }
    }

    protected override void Play(Vector2 position) {
        base.Play(position);

        if (positioningCardCoroutine != null) {
            OnStopPositioningCard();
        }

        if (IsModifiable) {
            AbilityManager.Instance.ApplyModifiers(this);
            DeckManager.Instance.DiscardStackedCards();
        }

        AbilityManager.Instance.AddActiveAbility(this);

        // only plays Stop method after duration if the ability card has duration, so if it doesn't
        // then the card is responsible for invoking base.Stop to remove the active ability
        bool hasDuration = abilityAttributes.HasFlag(AbilityAttribute.HasDuration);
        if (hasDuration) {
            durationStopCoroutine = AbilityManager.Instance.StartCoroutine(StopAfterDuration());
        }
    }

    public void Cancel() {
        if (positioningCardCoroutine != null) {
            OnStopPositioningCard();
        }
    }

    private IEnumerator StopAfterDuration() {
        yield return new WaitForSeconds(abilityStats.Duration);
        Stop();
    }

    public virtual void Stop() {
        AbilityManager.Instance.RemoveActiveAbility(this);
    }

    public virtual void AddEffect(GameObject effectPrefab) {
    }

    private Coroutine durationStopCoroutine;

    // this method is played instead of the Play() when it's already active (some cards can stack on themselves instead
    // of just extending the duration
    public void ResetDuration() {

        if (positioningCardCoroutine != null) {
            AbilityManager.Instance.StopCoroutine(positioningCardCoroutine);
        }

        if (IsModifiable) {
            AbilityManager.Instance.ApplyModifiers(this);
            DeckManager.Instance.DiscardStackedCards();
        }

        AbilityManager.Instance.StopCoroutine(durationStopCoroutine);
        durationStopCoroutine = AbilityManager.Instance.StartCoroutine(StopAfterDuration());
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
