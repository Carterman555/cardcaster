using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FireEffectOnDamage : MonoBehaviour, IAbilityEffect {

    private ITargetAttacker[] attackers;

    [SerializeField] private ParticleSystem fireAbilityParticles;
    [SerializeField] private float burnDuration;

    private void OnEnable() {
        attackers = transform.parent.GetComponents<ITargetAttacker>();

        foreach (ITargetAttacker attacker in attackers) {
            attacker.OnDamage_Target += InflictBurn;
        }

        // parent to biggest renderer in order to match the transform to match the sprite shape and make sure the
        // particles emit from the visual and move with it
        Transform parent = transform.parent.GetBiggestRenderer().transform;
        fireAbilityParticles.Spawn(parent);
    }

    private void OnDisable() {
        foreach (ITargetAttacker attacker in attackers) {
            attacker.OnDamage_Target -= InflictBurn;
        }

        fireAbilityParticles.gameObject.ReturnToPool();
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
