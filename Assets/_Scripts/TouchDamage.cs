using IslandDefender;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace IslandDefender.Management {

    [RequireComponent(typeof(TriggerContactTracker))]
    public class TouchDamage : MonoBehaviour {

        public event Action OnDamage;

        [SerializeField] private float damage = 1;
        [SerializeField] private float damageCooldown = 1f; // Damage interval in seconds
        [SerializeField] private bool applyKnockback = true;

        private TriggerContactTracker tracker;
        private Dictionary<GameObject, Coroutine> activeCoroutines = new Dictionary<GameObject, Coroutine>();

        private Health health;
        private bool dead;

        private void Awake() {
            tracker = GetComponent<TriggerContactTracker>();
            health = GetComponentInParent<Health>();
        }

        private void OnEnable() {
            tracker.OnEnterContact += HandleEnterContact;
            tracker.OnLeaveContact += HandleLeaveContact;

            if (health != null) {
                health.OnDeath += StopAllDamage;
            }

            ResetValues();
        }


        private void OnDisable() {
            tracker.OnEnterContact -= HandleEnterContact;
            tracker.OnLeaveContact -= HandleLeaveContact;
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

                if (target.TryGetComponent(out Health health)) {
                    if (applyKnockback) {
                        health.Damage(damage);
                    }

                    OnDamage?.Invoke();
                }
                //if (target.TryGetComponent(out Knockback knockback)) {

                //}

                yield return new WaitForSeconds(damageCooldown);
            }
        }
    }
}
