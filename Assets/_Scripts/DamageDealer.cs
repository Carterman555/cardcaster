using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer {

    public static void DealDamage(GameObject target, Vector2 attackerPos, float damage, float knockbackStrength) {
        if (target.TryGetComponent(out IDamagable damagable)) {
            damagable.Damage(damage);
        }
        if (target.TryGetComponent(out Knockback knockback)) {
            Vector2 toEnemyDirection = (Vector2)target.transform.position - attackerPos;
            knockback.ApplyKnockback(toEnemyDirection, knockbackStrength);
        }
    }

    public static Collider2D[] DealCircleDamage(LayerMask targetLayerMask, Vector2 attackCenter, float attackRadius, float damage, float knockbackStrength) {
        Collider2D[] cols = Physics2D.OverlapCircleAll(attackCenter, attackRadius, targetLayerMask);
        foreach (Collider2D col in cols) {
            DealDamage(col.gameObject, attackCenter, damage, knockbackStrength);
        }
        return cols;
    }
}
