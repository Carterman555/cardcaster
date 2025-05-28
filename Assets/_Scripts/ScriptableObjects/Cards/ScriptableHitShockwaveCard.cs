using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShockwaveCard", menuName = "Cards/Shockwave Card")]
public class ScriptableHitShockwave : ScriptableAbilityCardBase {

    [SerializeField] private CircleDamageAttacker shockwavePrefab;

    protected override void Play(Vector2 position) {
        base.Play(position);

        PlayerMeleeAttack.Instance.OnDamage_Target += CreateShockwave;
    }

    public override void Stop() {
        base.Stop();

        effectModifiers.Clear();

        PlayerMeleeAttack.Instance.OnDamage_Target -= CreateShockwave;
    }

    private void CreateShockwave(GameObject target) {
        CircleDamageAttacker circleDamage = shockwavePrefab.Spawn(target.transform.position, Containers.Instance.Effects);

        ApplyEffects(circleDamage.transform);

        circleDamage.DealDamage(GameLayers.EnemyLayerMask, Stats.AreaSize, Stats.Damage, Stats.KnockbackStrength);
    }

    private List<EffectModifier> effectModifiers = new();

    public override void ApplyModifier(ScriptableModifierCardBase modifierCard) {
        base.ApplyModifier(modifierCard);
        if (modifierCard.AppliesEffect) {
            effectModifiers.Add(modifierCard.EffectModifier);
        }
    }

    private void ApplyEffects(Transform attacker) {
        foreach (EffectModifier effectModifier in effectModifiers) {
            effectModifier.EffectLogicPrefab.Spawn(attacker);
        }
    }
}
