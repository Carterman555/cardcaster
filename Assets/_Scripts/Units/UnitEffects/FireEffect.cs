using System.Collections;
using UnityEngine;

public class FireEffect : UnitEffect {
    private IDamagable damagable;

    private ParticleSystem fireParticles;

    public override void Setup(bool removeAfterDuration = false, float duration = 0) {
        base.Setup(removeAfterDuration, duration);

        damagable = GetComponent<IDamagable>();
        fireParticles = GetComponentInChildren<UnitEffectVisuals>().AddParticleEffect(AssetSystem.Instance.UnitFireParticles);

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

    protected override void OnDisable() {
        base.OnDisable();
        if (fireParticles != null) {
            fireParticles.gameObject.TryReturnToPool();
        }
    }
}
