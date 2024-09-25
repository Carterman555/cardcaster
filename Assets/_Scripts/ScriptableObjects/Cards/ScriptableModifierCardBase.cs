using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ModifierCard", menuName = "Cards/Modifiers/Base")]
public class ScriptableModifierCardBase : ScriptableCardBase {

    [SerializeField] private AbilityAttribute abilityAttributes;
    public AbilityAttribute AbilityAttributes => abilityAttributes;

    [SerializeField] private AbilityStats abilityStatsModifierPercentage;
    public AbilityStats StatsModifier => abilityStatsModifierPercentage;

    [SerializeField] private bool canStackWithSelf;
    public bool CanStackWithSelf => canStackWithSelf;

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

        if (appliesEffect) {
            card.AddEffect(effectPrefab);
        }
    }
}


