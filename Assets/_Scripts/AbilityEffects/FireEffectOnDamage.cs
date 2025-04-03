using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FireEffectOnDamage : MonoBehaviour, IAbilityEffect {

    [SerializeField] private ParticleSystem fireAbilityParticlesPrefab;
    [SerializeField] private float burnDuration;

    private EffectOnDamage effectOnDamage;

    private void OnEnable() {
        StartCoroutine(OnEnableCor());
    }

    private IEnumerator OnEnableCor() {

        //... wait a frame before checking for attackers because an attacker could have been added
        //... in the same frame. Like if they were both added from an ability card. This makes sure
        //... this subscribes to the attackers' onDamage event
        yield return null;

        effectOnDamage = new(InflictBurn, transform.parent, fireAbilityParticlesPrefab);
    }

    private void OnDisable() {
        effectOnDamage.Disable();
    }

    private void InflictBurn(GameObject target) {

        if (target.TryGetComponent(out IDamagable damagable) && damagable.Dead) {
            return;
        }

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
