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
    }

    private void OnDestroy() {
        GetComponentInChildren<UnitEffectVisuals>().RemoveParticleEffect(fireParticles);
    }

    protected override void Update() {
        base.Update();

        float damagePerSecond = 2f;
        health.Damage(damagePerSecond * Time.deltaTime);
    }
}
