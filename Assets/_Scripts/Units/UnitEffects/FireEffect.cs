using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FireEffect : UnitEffect {
    private Health health;

    private ParticleSystem fireParticles;

    public override void Setup(bool removeAfterDuration = false, float duration = 0) {
        base.Setup(removeAfterDuration, duration);

        health = GetComponent<Health>();
        fireParticles = GetComponentInChildren<UnitEffectVisuals>().AddParticleEffect(AssetSystem.Instance.UnitFireParticles);

        StartCoroutine(Burn());
    }

    private void OnDestroy() {
        GetComponentInChildren<UnitEffectVisuals>().RemoveParticleEffect(fireParticles);
    }

    private IEnumerator Burn() {
        while (enabled) {
            yield return new WaitForSeconds(1f);

            float damagePerSecond = 2f;
            health.Damage(damagePerSecond);
        }
    }
}
