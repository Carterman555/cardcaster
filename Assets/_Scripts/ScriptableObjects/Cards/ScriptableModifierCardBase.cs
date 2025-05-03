using UnityEngine;

[CreateAssetMenu(fileName = "ModifierCard", menuName = "Cards/Modifiers/Base")]
public class ScriptableModifierCardBase : ScriptableCardBase {

    [SerializeField] private AbilityAttribute abilityAttributes;
    public AbilityAttribute AbilityAttributes => abilityAttributes;

    [SerializeField] private AbilityStats abilityStatsModifierPercentage;
    public AbilityStats StatsModifier => abilityStatsModifierPercentage;

    [SerializeField] private bool appliesEffect;
    [ConditionalHide("appliesEffect")][SerializeField] private GameObject effectPrefab;

    public override void TryPlay(Vector2 position) {
        base.TryPlay(position);
        Play(position);
    }

    protected override void Play(Vector2 position) {
        base.Play(position);
        AbilityManager.Instance.AddModifier(this);
    }

    public virtual void ApplyToAbility(ScriptableAbilityCardBase card) {

        // apply stats modifier
        //... the attributes that both the ability card and modifier card share
        AbilityAttribute abilityAttributesToModify = card.AbilityAttributes & abilityAttributes;

        if (!appliesEffect) effectPrefab = null;
        card.ApplyModifier(StatsModifier, abilityAttributesToModify, effectPrefab);
    }
}


