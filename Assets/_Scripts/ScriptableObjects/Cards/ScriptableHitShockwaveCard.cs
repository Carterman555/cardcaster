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

        effectPrefabs.Clear();

        PlayerMeleeAttack.Instance.OnDamage_Target -= CreateShockwave;
    }

    private void CreateShockwave(GameObject target) {
        CircleDamageAttacker circleDamage = shockwavePrefab.Spawn(target.transform.position, Containers.Instance.Effects);

        ApplyEffects(circleDamage.transform);

        circleDamage.DealDamage(GameLayers.EnemyLayerMask, Stats.AreaSize, Stats.Damage, Stats.KnockbackStrength);
    }

    private List<GameObject> effectPrefabs = new();

    public override void ApplyModifier(AbilityStats statsModifier, AbilityAttribute abilityAttributesToModify, GameObject effectPrefab) {
        base.ApplyModifier(statsModifier, abilityAttributesToModify, effectPrefab);
        if (effectPrefab != null) {
            effectPrefabs.Add(effectPrefab);
        }
    }

    private void ApplyEffects(Transform attacker) {
        foreach (GameObject effectPrefab in effectPrefabs) {
            effectPrefab.Spawn(attacker);
        }
    }
}
