using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InflictFireDamage : MonoBehaviour, IEffect {
    private Health health;

    private int particleId;

    private void Setup(Enemy enemy, bool removeAfterDuration = false, float duration = 0) {

        health = enemy.GetComponent<Health>();

        particleId = enemy.GetComponentInChildren<UnitEffectVisuals>().AddParticleEffect(AssetSystem.Instance.UnitFireParticles);
    }

    //public override void OnEffectRemoved() {
    //    base.OnEffectRemoved();

    //    enemy.GetComponentInChildren<UnitEffectVisuals>().RemoveParticleEffect(particleId);
    //}

    //public override void UpdateLogic() {
    //    base.UpdateLogic();

    //    float damagePerSecond = 0.5f;
    //    health.Damage(damagePerSecond * Time.deltaTime);
    //}

    public void Apply(GameObject target) {
        //throw new System.NotImplementedException();
    }
}
