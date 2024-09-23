using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ScriptableModifierCardBase : ScriptableCardBase {

    [SerializeField] private AbilityAttribute abilityAttributes;
    public AbilityAttribute AbilityAttributes => abilityAttributes;

    [SerializeField] private AbilityStats abilityStatsModifier;
    public AbilityStats StatsModifier => abilityStatsModifier;

    [SerializeField] private bool hasVisualEffect;
    [ConditionalHide("hasVisualEffect")][SerializeField] private Transform visualEffect;

    [SerializeField] private bool appliesEffect;
    [ConditionalHide("appliesEffect")][SerializeField] private GameObject effectPrefab;

    public override void Play(Vector2 position) {
        base.Play(position);

        AbilityManager.Instance.AddModifier(this);
    }

    public virtual void ApplyToAbility(ScriptableAbilityCardBase card) {

        // apply stats modifier
        //... the attributes that both the ability card and modifier card share
        AbilityAttribute abilityAttributesToModify = card.AbilityAttributes & abilityAttributes;
        card.Stats.ApplyModifier(StatsModifier, abilityAttributesToModify);

        if (hasVisualEffect) {
            card.TryApplyVisualEffect(visualEffect);
        }

        if (appliesEffect) {
            card.AddEffect(effectPrefab);
        }
    }
}


