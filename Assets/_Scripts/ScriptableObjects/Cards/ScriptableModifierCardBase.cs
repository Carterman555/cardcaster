using UnityEngine;

[CreateAssetMenu(fileName = "ModifierCard", menuName = "Cards/Modifiers/Base")]
public class ScriptableModifierCardBase : ScriptableCardBase {

    [SerializeField] private AbilityAttribute abilityAttributes;
    public AbilityAttribute AbilityAttributes => abilityAttributes;

    [SerializeField] private AbilityStats abilityStatsModifierPercentage;
    public AbilityStats StatsModifier => abilityStatsModifierPercentage;

    [SerializeField] private ModifierImage modifierImagePrefab;

    [SerializeField] private bool appliesEffect;
    [ConditionalHide("appliesEffect")][SerializeField] private GameObject effectPrefab;

    public override void TryPlay(Vector2 position) {
        base.TryPlay(position);
        Play(position);
    }

    protected override void Play(Vector2 position) {
        base.Play(position);

        if (StackType == StackType.Stackable || !AbilityManager.Instance.IsModifierActive(this)) {
            Vector2 canvasPos = Camera.main.WorldToScreenPoint(position);
            ModifierImage modifierImage = modifierImagePrefab.Spawn(canvasPos, Containers.Instance.HUD);
            modifierImage.Setup(this);
        }

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


