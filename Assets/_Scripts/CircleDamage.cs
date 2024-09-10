using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleDamage {

    public static Collider2D[] DealDamage(LayerMask targetLayerMask, Vector2 attackCenter, float attackRadius, float damage, float knockbackStrength) {
        Collider2D[] cols = Physics2D.OverlapCircleAll(attackCenter, attackRadius, targetLayerMask);
        foreach (Collider2D col in cols) {
            if (col.TryGetComponent(out IDamagable damagable)) {
                damagable.Damage(damage);
            }
            if (col.TryGetComponent(out Knockback knockback)) {
                Vector2 toEnemyDirection = (Vector2)col.transform.position - attackCenter;
                knockback.ApplyKnockback(toEnemyDirection, knockbackStrength);
            }
        }
        return cols;
    }
}
