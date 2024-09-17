using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnContact : MonoBehaviour, ITargetAttacker {

    public event Action OnAttack;
    public event Action<GameObject> OnDamage_Target;

    [SerializeField] private LayerMask targetLayer;

    private IDelayedReturn[] delayedReturns;

    private float damage;
    private float knockbackStrength;

    private bool canDamage;

    private void Awake() {
        delayedReturns = GetComponents<IDelayedReturn>();
    }

    public void Setup(float damage, float knockbackStrength) {
        this.damage = damage;
        this.knockbackStrength = knockbackStrength;

        canDamage = true;
    }

    private void OnTriggerEnter2D(Collider2D collision) {

        if (!canDamage) {
            return;
        }

        if (targetLayer.ContainsLayer(collision.gameObject.layer)) {
            if (collision.TryGetComponent(out IDamagable damagable)) {
                damagable.Damage(damage);

                OnDamage_Target?.Invoke(collision.gameObject);
            }
            if (collision.TryGetComponent(out Knockback knockback)) {
                Vector2 toTargetDirection = collision.transform.position - transform.position;
                knockback.ApplyKnockback(toTargetDirection, knockbackStrength);
            }

            PreventDamage();

            OnAttack?.Invoke();
        }
    }

    // whenever a behavior starts to return this object (the death animation starts), stop it from dealing damage
    private void OnEnable() {
        foreach (var delayedReturn in delayedReturns) {
            delayedReturn.OnStartReturn += PreventDamage;
        }
    }
    private void OnDisable() {
        foreach (var delayedReturn in delayedReturns) {
            delayedReturn.OnStartReturn -= PreventDamage;
        }
    }

    public void PreventDamage() {
        canDamage = false;
    }
}
