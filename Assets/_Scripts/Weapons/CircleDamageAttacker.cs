using System;
using UnityEngine;

public class CircleDamageAttacker : MonoBehaviour, ITargetAttacker {

    public event Action<GameObject> OnDamage_Target;
    public event Action OnAttack;

    public void DealDamage(LayerMask targetLayerMask, float attackRadius, float damage, float knockbackStrength) {
        Collider2D[] targets = DamageDealer.DealCircleDamage(targetLayerMask, transform.position, transform.position, attackRadius, damage, knockbackStrength);

        OnAttack?.Invoke();
        foreach (Collider2D target in targets) {
            OnDamage_Target?.Invoke(target.gameObject);
        }
    }
}
