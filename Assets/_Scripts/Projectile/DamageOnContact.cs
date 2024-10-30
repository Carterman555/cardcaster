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

    [SerializeField] private bool piercing;
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

            bool dealtDamage = DamageDealer.TryDealDamage(
                collision.gameObject,
                transform.position,
                damage,
                knockbackStrength);

            if (dealtDamage) {
                OnDamage_Target?.Invoke(collision.gameObject);
            }

            if (!piercing) {
                PreventDamage();
            }

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
