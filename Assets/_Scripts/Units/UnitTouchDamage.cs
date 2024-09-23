using IslandDefender;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;


namespace IslandDefender.Management {

    [RequireComponent(typeof(TriggerContactTracker))]
    public class UnitTouchDamage : MonoBehaviour {

        private TriggerContactTracker tracker;
        private Dictionary<GameObject, Coroutine> activeCoroutines = new Dictionary<GameObject, Coroutine>();

        private Health health;
        private bool dead;

        private IHasStats hasStats;

        private void Awake() {
            tracker = GetComponent<TriggerContactTracker>();
            health = GetComponentInParent<Health>();
            hasStats = GetComponentInParent<IHasStats>();
        }

        private void OnEnable() {
            tracker.OnEnterContact += HandleEnterContact;
            tracker.OnExitContact += HandleLeaveContact;

            if (health != null) {
                health.OnDeath += StopAllDamage;
            }

            ResetValues();
        }


        private void OnDisable() {
            tracker.OnEnterContact -= HandleEnterContact;
            tracker.OnExitContact -= HandleLeaveContact;
        }

        private void ResetValues() {
            activeCoroutines.Clear();
            dead = false;
        }

        private void HandleEnterContact(GameObject target) {
            if (!activeCoroutines.ContainsKey(target) && !dead) {
                Coroutine coroutine = StartCoroutine(DamageOverTime(target));
                activeCoroutines[target] = coroutine;
            }
        }

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
        }

        private IEnumerator DamageOverTime(GameObject target) {
            while (true) {

                if (!target.activeSelf) {
                    if (activeCoroutines.TryGetValue(target, out Coroutine coroutine)) {
                        StopCoroutine(coroutine);
                        activeCoroutines.Remove(target);
                    }
                }

                DamageDealer.TryDealDamage(target,
                    transform.position,
                    hasStats.GetStats().Damage,
                    hasStats.GetStats().KnockbackStrength);

                yield return new WaitForSeconds(hasStats.GetStats().AttackCooldown);
            }
        }
    }
}
