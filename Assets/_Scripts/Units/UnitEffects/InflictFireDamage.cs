using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InflictFireDamage : UnitEffect {
    private Health health;

    private int particleId;

    public override System.Guid Setup(bool removeAfterDuration = false, float duration = 0) {
        System.Guid id = base.Setup(removeAfterDuration, duration);

        health = GetComponent<Health>();
        particleId = GetComponentInChildren<UnitEffectVisuals>().AddParticleEffect(AssetSystem.Instance.UnitFireParticles);

        return id;
    }

    private void OnDestroy() {
        GetComponentInChildren<UnitEffectVisuals>().RemoveParticleEffect(particleId);
    }

    protected override void Update() {
        base.Update();

        float damagePerSecond = 0.5f;
        health.Damage(damagePerSecond * Time.deltaTime);
    }
}
