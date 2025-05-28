using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DamageOnContact : MonoBehaviour, ITargetAttacker {

    public event Action OnAttack;
    public event Action<GameObject> OnDamage_Target;

    [SerializeField] private LayerMask targetLayer;

    private float damage;
    private float knockbackStrength;
    private bool canCrit;

    [SerializeField] private bool piercing;
    private bool dealtDamage;

    private struct TargetTimePair {
        public Transform Target;
        public float Time;
    }

    private List<TargetTimePair> recentTargets;

    public void Setup(float damage, float knockbackStrength, bool canCrit = false) {
        this.damage = damage;
        this.knockbackStrength = knockbackStrength;
        this.canCrit = canCrit;

        dealtDamage = false;

        recentTargets = new();
    }

    private void OnTriggerEnter2D(Collider2D collision) {

        if (!enabled) {
            return;
        }

        if (!piercing && dealtDamage) {
            return;
        }

        bool recentlyDamagedTarget = recentTargets.Any(p => p.Target == collision.transform);
        if (piercing && recentlyDamagedTarget) {
            return;
        }

        if (targetLayer.ContainsLayer(collision.gameObject.layer)) {

            bool dealtDamage = DamageDealer.TryDealDamage(
                collision.gameObject,
                transform.position,
                damage,
                knockbackStrength,
                canCrit);

            if (dealtDamage) {
                TargetTimePair targetTimePair = new() {
                    Target = collision.transform,
                    Time = Time.time
                };
                recentTargets.Add(targetTimePair);

                OnDamage_Target?.Invoke(collision.gameObject);
            }

            this.dealtDamage = true;

            OnAttack?.Invoke();
        }
    }

    private void Update() {

        if (recentTargets == null) {
            Debug.LogError($"DamageOnContact has not been setup for {name}!");
            return;
        }

        for (int i = recentTargets.Count - 1; i >= 0; i--) {
            float attackAgainCooldown = 0.3f;
            if (recentTargets[i].Time + attackAgainCooldown < Time.time) {
                recentTargets.RemoveAt(i);
            }
        }
    }
}
