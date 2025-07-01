using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ShockwaveCard", menuName = "Cards/Shockwave Card")]
public class ScriptableHitShockwave : ScriptableAbilityCardBase {

    [SerializeField] private CircleDamageAttacker shockwavePrefab;

    protected override void Play(Vector2 position) {
        base.Play(position);

        PlayerMeleeAttack.Instance.OnAttack_Targets += TryCreateShockwave;
    }

    public override void Stop() {
        base.Stop();

        effectModifiers.Clear();

        PlayerMeleeAttack.Instance.OnAttack_Targets -= TryCreateShockwave;
    }

    private void TryCreateShockwave(GameObject[] targets) {

        if (targets.Length == 0) {
            return;
        }

        Vector2 pos = targets.Select(t => t.transform.position).ToArray().GetAveragePos();
        CircleDamageAttacker circleDamage = shockwavePrefab.Spawn(pos, Containers.Instance.Projectiles);

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
