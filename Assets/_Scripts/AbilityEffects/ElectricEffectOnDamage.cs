using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricEffectOnDamage : MonoBehaviour, IAbilityEffect {
    private ITargetAttacker[] attackers;

    [SerializeField] private ParticleSystem electricAbilityParticlesPrefab;
    private ParticleSystem electricAbilityParticles;

    [SerializeField] private float damage;
    [SerializeField] private int chainSize;

    private void OnEnable() {
        attackers = transform.parent.GetComponents<ITargetAttacker>();

        foreach (ITargetAttacker attacker in attackers) {
            attacker.OnDamage_Target += Shock;
        }

        // parent to biggest renderer in order to match the transform to match the sprite shape and make sure the
        // particles emit from the visual and move with it
        Transform parent = transform.parent.GetBiggestRenderer().transform;
        electricAbilityParticles = electricAbilityParticlesPrefab.Spawn(parent);
    }

    private void OnDisable() {
        foreach (ITargetAttacker attacker in attackers) {
            attacker.OnDamage_Target -= Shock;
        }

        electricAbilityParticles.gameObject.ReturnToPool();
    }

    private void Shock(GameObject target) {
        if (target.TryGetComponent(out IEffectable effectable)) {
            ElectricChain electricChain = target.AddComponent<ElectricChain>();
            electricChain.Setup(damage, chainSize);
        }
    }
}
