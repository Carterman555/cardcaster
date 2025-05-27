using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ModifierCard", menuName = "Cards/Modifiers/Base")]
public class ScriptableModifierCardBase : ScriptableCardBase {

    [SerializeField] private AbilityAttribute abilityAttributes;
    public AbilityAttribute AbilityAttributes => abilityAttributes;

    [SerializeField] private AbilityStats abilityStatsModifierPercentage;
    public AbilityStats StatsModifier => abilityStatsModifierPercentage;

    [SerializeField] private bool appliesEffect;
    public bool AppliesEffect => appliesEffect;

    [ConditionalHide("appliesEffect")][SerializeField] private EffectModifier effectModifier;
    public EffectModifier EffectModifier => effectModifier;

    public override void TryPlay(Vector2 position) {
        base.TryPlay(position);
        Play(position);
    }

    protected override void Play(Vector2 position) {
        base.Play(position);
        AbilityManager.Instance.AddModifier(this);
    }
}

[Serializable]
public struct EffectModifier {
    public GameObject EffectLogicPrefab;

    public bool HasVisual;
    public GameObject EffectVisualPrefab;
}


