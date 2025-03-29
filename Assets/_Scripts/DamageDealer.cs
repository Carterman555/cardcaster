using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer {

    public static bool TryDealDamage(GameObject target, Vector2 attackerPos, float damage, float knockbackStrength, bool canCrit = false) {
        bool dealtDamage = false;
        if (target.TryGetComponent(out IDamagable damagable)) {

            bool crit = canCrit && StatsManager.PlayerStats.CritChance > Random.Range(0f, 1f);
            if (crit) {
                damage *= StatsManager.PlayerStats.CritDamageMult;
            }

            damagable.Damage(damage, crit: crit);
            dealtDamage = true;
        }
        if (target.TryGetComponent(out Knockback knockback)) {
            Vector2 toEnemyDirection = (Vector2)target.transform.position - attackerPos;
            knockback.ApplyKnockback(toEnemyDirection, knockbackStrength);
        }
        return dealtDamage;
    }

    public static Collider2D[] DealCircleDamage(LayerMask targetLayerMask, Vector2 attackCenter, float attackRadius, float damage, float knockbackStrength, bool canCrit = false) {
        Collider2D[] cols = Physics2D.OverlapCircleAll(attackCenter, attackRadius, targetLayerMask);
        foreach (Collider2D col in cols) {
            TryDealDamage(col.gameObject, attackCenter, damage, knockbackStrength, canCrit);
        }
        return cols;
    }

    public static Collider2D[] DealCapsuleDamage(LayerMask targetLayerMask, Vector2 attackCenter, Vector2 size, float angle, float damage, float knockbackStrength, bool canCrit = false) {
        Collider2D[] cols = Physics2D.OverlapCapsuleAll(attackCenter, size, CapsuleDirection2D.Horizontal, angle, targetLayerMask);
        foreach (Collider2D col in cols) {
            TryDealDamage(col.gameObject, attackCenter, damage, knockbackStrength, canCrit);
        }
        return cols;
    }
}
