using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class ScriptableAbilityCardBase : ScriptableCardBase {

    [SerializeField] private AbilityAttribute abilityAttributes;
    public AbilityAttribute AbilityAttributes => abilityAttributes;

    [FormerlySerializedAs("abilityStats")]
    [SerializeField] private AbilityStats baseStats;
    public AbilityStats Stats { get; private set; }

    [Tooltip("If true, this will apply damage and knockback modifiers to player stats")]
    [SerializeField] private bool modifiesSword;
    private List<PlayerStatModifier> appliedPlayerStatModifiers = new();

    [field: SerializeField] public bool IsPositional { get; private set; }
    [field: SerializeField] public bool IsModifiable { get; private set; } = true;

    [SerializeField] private CardType[] incompatibleAbilities;
    public CardType[] IncompatibleAbilities => incompatibleAbilities;

    public static event Action OnStartPositioning;
    public static event Action OnStopPositioning;
    private Coroutine positioningCardCoroutine;

    public bool IsCompatibleWithModifier(ScriptableModifierCardBase modifier) {
        bool anyFlagsMatch = (abilityAttributes & modifier.AbilityAttributes) != 0;
        return anyFlagsMatch;
    }

    public override void OnInstanceCreated() {
        base.OnInstanceCreated();
        positioningCardCoroutine = null;
    }

    public virtual void OnStartPositioningCard(Transform cardTransform) {
        positioningCardCoroutine = AbilityManager.Instance.StartCoroutine(PositioningCard(cardTransform));
        OnStartPositioning?.Invoke();
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
        OnStopPositioning?.Invoke();
    }

    public override void TryPlay(Vector2 position) {
        base.TryPlay(position);

        if (!CanPlay()) {
            return;
        }

        bool alreadyActive = AbilityManager.Instance.IsAbilityActive(this);

        if (!alreadyActive) {
            Play(position);
        }
        else if (alreadyActive) {

            // if multiple can't play at the same time, reset the duration of the current one playing or don't allow playing
            if (StackType == StackType.Stackable) {
                Play(position);
            }
            else if (StackType == StackType.ResetDuration) {
                AbilityManager.Instance.IsAbilityActive(this, out ScriptableAbilityCardBase alreadyActiveAbility);
                alreadyActiveAbility.ResetDuration();
            }
        }
    }

    protected override void Play(Vector2 position) {
        base.Play(position);

        Stats = baseStats;

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

            AbilityIndicatorManager.Instance.AddIndicator(this);
        }
    }

    // checks if this card is compatible to play with cards that are currently active. Well, right now it 
    // only checks if can stack with self and it's active (not other cards yet)
    public override bool CanPlay() {

        if (IsIncompatibleAbilityActive()) {
            return false;
        }

        bool canStack = StackType != StackType.NotStackable;
        bool alreadyActive = AbilityManager.Instance.IsAbilityActive(this);

        return canStack || !alreadyActive;
    }

    public bool IsIncompatibleAbilityActive() {
        foreach (CardType cardType in incompatibleAbilities) {
            bool incompatibleAbilityActive = AbilityManager.Instance.IsAbilityActive(cardType);
            if (incompatibleAbilityActive) {
                return true;
            }
        }
        return false;
    }

    public void Cancel() {
        if (positioningCardCoroutine != null) {
            OnStopPositioningCard();
        }
    }

    private IEnumerator StopAfterDuration() {
        yield return new WaitForSeconds(Stats.Duration);
        Stop();
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

        AbilityIndicatorManager.Instance.ResetDurationOfIndicator(this);
    }

    public virtual void Stop() {
        AbilityManager.Instance.RemoveActiveAbility(this);

        if (modifiesSword) {
            StatsManager.RemovePlayerStatModifiers(appliedPlayerStatModifiers.ToArray());
            appliedPlayerStatModifiers.Clear();
        }
    }

    public virtual void ApplyModifier(AbilityStats statsModifier, AbilityAttribute abilityAttributesToModify, GameObject effectPrefab) {
        AbilityStats newStats = Stats;

        if (abilityAttributesToModify.HasFlag(AbilityAttribute.DealsDamage)) {
            newStats.BaseDamage *= statsModifier.Damage.PercentToMult();
            newStats.KnockbackStrength *= statsModifier.KnockbackStrength.PercentToMult();

            if (modifiesSword) {
                PlayerStatModifier damageModifier = new() {
                    Value = statsModifier.Damage.PercentToMult(),
                    PlayerStatType = PlayerStatType.Damage,
                    ModifyType = ModifyType.Multiplicative
                };
                PlayerStatModifier dashDamageModifier = new() {
                    Value = statsModifier.Damage.PercentToMult(),
                    PlayerStatType = PlayerStatType.DashAttackDamage,
                    ModifyType = ModifyType.Multiplicative
                };
                PlayerStatModifier knockbackModifier = new() {
                    Value = statsModifier.KnockbackStrength.PercentToMult(),
                    PlayerStatType = PlayerStatType.KnockbackStrength,
                    ModifyType = ModifyType.Multiplicative
                };

                StatsManager.AddPlayerStatModifier(damageModifier);
                StatsManager.AddPlayerStatModifier(dashDamageModifier);
                StatsManager.AddPlayerStatModifier(knockbackModifier);

                appliedPlayerStatModifiers.Add(damageModifier);
                appliedPlayerStatModifiers.Add(dashDamageModifier);
                appliedPlayerStatModifiers.Add(knockbackModifier);
            }
        }
        if (abilityAttributesToModify.HasFlag(AbilityAttribute.HasArea)) {
            newStats.AreaSize *= statsModifier.AreaSize.PercentToMult();
        }
        if (abilityAttributesToModify.HasFlag(AbilityAttribute.HasDuration)) {
            newStats.Duration *= statsModifier.Duration.PercentToMult();
        }
        if (abilityAttributesToModify.HasFlag(AbilityAttribute.IsProjectile)) {
            newStats.ProjectileSpeed *= statsModifier.ProjectileSpeed.PercentToMult();
        }
        if (abilityAttributesToModify.HasFlag(AbilityAttribute.HasCooldown)) {
            newStats.Cooldown *= statsModifier.Cooldown.PercentToMult();
        }

        Stats = newStats;
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
public struct AbilityStats {
    [ConditionalHideFlag("abilityAttributes", AbilityAttribute.DealsDamage)]
    public float BaseDamage;
    public float Damage => BaseDamage * StatsManager.PlayerStats.AllDamageMult;

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
}
