using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

[RequireComponent(typeof(ExplodeBehavior))]
public class Bomb : MonoBehaviour, IAbilityStatsSetup {

    private ExplodeBehavior explodeBehavior;

    private void Awake() {
        explodeBehavior = GetComponent<ExplodeBehavior>();
    }

    public void SetAbilityStats(AbilityStats stats) {
        explodeBehavior.SetDamage(stats.Damage);
        explodeBehavior.SetExplosionRadius(stats.AreaSize);
        explodeBehavior.SetKnockbackStrength(stats.KnockbackStrength);

        StartCoroutine(DelayedExplode(stats.Cooldown));
    }

    public IEnumerator DelayedExplode(float delay) {
        yield return new WaitForSeconds(delay);
        explodeBehavior.Explode();
    }
}
