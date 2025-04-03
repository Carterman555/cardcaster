using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FireEffect : UnitEffect {
    private IDamagable damagable;

    public override void Setup(bool removeAfterDuration = false, float duration = 0) {
        base.Setup(removeAfterDuration, duration);

        damagable = GetComponent<IDamagable>();
        GetComponentInChildren<UnitEffectVisuals>().AddParticleEffect(AssetSystem.Instance.UnitFireParticles);

        StartCoroutine(Burn());
    }

    private IEnumerator Burn() {
        while (enabled) {
            yield return new WaitForSeconds(1f);

            float damagePerSecond = 2f;
            if (damagable is EnemyHealth) {
                damagePerSecond *= StatsManager.PlayerStats.AllDamageMult;
            }
            damagable.Damage(damagePerSecond);
        }
    }
}
