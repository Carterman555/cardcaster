using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FireEffectOnDamage : MonoBehaviour, IAbilityEffect {

    private ITargetAttacker[] attackers;

    private float burnDuration = ScriptableFireSwordCard.BurnDuration;

    public void SetBurnDuration(float burnDuration) {
        burnDuration = this.burnDuration;
    }

    private void OnEnable() {
        attackers = gameObject.GetComponents<ITargetAttacker>();

        foreach (ITargetAttacker attacker in attackers) {
            attacker.OnDamage_Target += InflictBurn;
        }

        // setup fire visual effects
        AssetSystem.Instance.AbilityFireParticles.Spawn(transform);
    }

    private void OnDisable() {
        foreach (ITargetAttacker attacker in attackers) {
            attacker.OnDamage_Target -= InflictBurn;
        }

        Destroy(this);
    }

    private void InflictBurn(GameObject target) {
        if (target.TryGetComponent(out IEffectable effectable)) {
            float burnDuration = 1.5f;
            effectable.AddEffect(new Burn(), true, burnDuration);
        }
    }
}
