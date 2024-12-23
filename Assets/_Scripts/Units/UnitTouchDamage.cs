using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;


[RequireComponent(typeof(TriggerContactTracker))]
public class UnitTouchDamage : MonoBehaviour {

    private TriggerContactTracker tracker;
    private Dictionary<GameObject, Coroutine> activeCoroutines = new Dictionary<GameObject, Coroutine>();

    //... the player has a period of invincibility after taking damage (right now it's 1.2s), so this might not be the actual
    //... attack cooldown
    [SerializeField] private float attackCooldown = 0.2f;

    [SerializeField] private bool hasHealth;
    [ConditionalHide("hasHealth")] [SerializeField] private Health health;
    private bool dead;

    [SerializeField] private bool overrideDamage;
    [ConditionalHide("overrideDamage")][SerializeField] private float damage;

    [SerializeField] private bool overrideKnockback;
    [ConditionalHide("overrideKnockback")][SerializeField] private float knockbackStrength;

    private IHasStats hasStats;

    private void Awake() {
        tracker = GetComponent<TriggerContactTracker>();

        if (health) {
            health = GetComponentInParent<Health>();
        }

        hasStats = GetComponentInParent<IHasStats>();
    }

    private void OnEnable() {
        tracker.OnEnterContact_GO += HandleEnterContact;
        tracker.OnExitContact_GO += HandleLeaveContact;

        if (hasHealth) {
            health.OnDeath += StopAllDamage;
        }

        activeCoroutines.Clear();
        dead = false;
    }


    private void OnDisable() {
        tracker.OnEnterContact_GO -= HandleEnterContact;
        tracker.OnExitContact_GO -= HandleLeaveContact;

        if (!hasHealth) {
            StopAllDamage();
        }
    }

    // if not already attacking the target and not dead, start a coroutine to attack
    private void HandleEnterContact(GameObject target) {
        if (!activeCoroutines.ContainsKey(target) && !dead) {
            Coroutine coroutine = StartCoroutine(DamageOverTime(target));
            activeCoroutines[target] = coroutine;
        }
    }

    // if attacking the target and not dead, stop the coroutine that's attacking
    private void HandleLeaveContact(GameObject target) {
        if (activeCoroutines.TryGetValue(target, out Coroutine coroutine) && !dead) {
            StopCoroutine(coroutine);
            activeCoroutines.Remove(target);
        }
    }

    private void StopAllDamage() {
        StopAllCoroutines();
        activeCoroutines.Clear();
        dead = true;

        if (hasHealth) {
            health.OnDeath -= StopAllDamage;
        }
    }

    private IEnumerator DamageOverTime(GameObject target) {
        while (true) {

            if (target == null) {
                yield break; // exit the coroutine
            }

            // if the target becomes inactive, stop attacking
            if (!target.activeSelf) {
                if (activeCoroutines.TryGetValue(target, out Coroutine coroutine)) {
                    StopCoroutine(coroutine);
                    activeCoroutines.Remove(target);
                }
            }

            float dmg = overrideDamage ? damage : hasStats.GetStats().Damage;
            float knockback = overrideKnockback ? knockbackStrength : hasStats.GetStats().KnockbackStrength;

            DamageDealer.TryDealDamage(target, transform.position, dmg, knockback);

            yield return new WaitForSeconds(attackCooldown);
        }
    }
}
