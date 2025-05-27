using System.Collections;
using UnityEngine;

public class FireEffectOnDamage : MonoBehaviour, IAbilityEffect {

    [SerializeField] private float burnDuration;

    private Coroutine onEnabledCor;

    private ITargetAttacker[] attackers = new ITargetAttacker[0];

    private void OnEnable() {
        attackers = transform.parent.GetComponentsInChildren<ITargetAttacker>();

        foreach (ITargetAttacker attacker in attackers) {
            attacker.OnDamage_Target += InflictBurn;
        }
    }

    private void OnDisable() {
        if (onEnabledCor != null) {
            StopCoroutine(onEnabledCor);
        }

        foreach (ITargetAttacker attacker in attackers) {
            attacker.OnDamage_Target -= InflictBurn;
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
