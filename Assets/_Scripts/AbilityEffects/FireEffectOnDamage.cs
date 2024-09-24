using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FireEffectOnDamage : MonoBehaviour, IAbilityEffect {

    private ITargetAttacker[] attackers;

    [SerializeField] private float burnDuration;

    private void OnEnable() {
        attackers = transform.parent.GetComponents<ITargetAttacker>();

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
    }

    private void InflictBurn(GameObject target) {
        if (target.TryGetComponent(out IEffectable effectable)) {

            // if the unit is already on fire don't add effect, but instead reset the time
            if (target.TryGetComponent(out FireEffect existingFireEffect)) {
                existingFireEffect.SetDuration(burnDuration);
            }
            else {
                FireEffect newFireEffect = target.AddComponent<FireEffect>();
                newFireEffect.Setup(true, burnDuration);
            }
            
        }
    }
}
