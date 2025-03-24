using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnContact : MonoBehaviour, ITargetAttacker {

    public event Action OnAttack;
    public event Action<GameObject> OnDamage_Target;

    [SerializeField] private LayerMask targetLayer;

    private float damage;
    private float knockbackStrength;
    private bool canCrit;

    [SerializeField] private bool piercing;
    private bool canDamage;

    public void Setup(float damage, float knockbackStrength, bool canCrit = false) {
        this.damage = damage;
        this.knockbackStrength = knockbackStrength;
        this.canCrit = canCrit;

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
                knockbackStrength,
                canCrit);

            if (dealtDamage) {
                OnDamage_Target?.Invoke(collision.gameObject);
            }

            if (!piercing) {
                canDamage = false;
            }

            OnAttack?.Invoke();
        }
    }
}
