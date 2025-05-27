using System.Collections;
using UnityEngine;

public class FireEffectOnDamage : MonoBehaviour, IAbilityEffect {

    [SerializeField] private ParticleSystem fireAbilityParticlesPrefab;
    [SerializeField] private float burnDuration;

    private EffectOnDamage effectOnDamage;

    private Coroutine onEnabledCor;

    private void OnEnable() {
        onEnabledCor = StartCoroutine(OnEnableCor());
    }

    private IEnumerator OnEnableCor() {

        //... wait a frame before checking for attackers because an attacker could have been added
        //... in the same frame. Like if they were both added from an ability card. This makes sure
        //... this subscribes to the attackers' onDamage event
        yield return null;

        effectOnDamage = new(InflictBurn, attackerTransform: transform.parent, fireAbilityParticlesPrefab);
    }

    private void OnDisable() {
        if (onEnabledCor != null) {
            StopCoroutine(onEnabledCor);
        }
        if (effectOnDamage != null) {
            effectOnDamage.Disable();
        }
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
