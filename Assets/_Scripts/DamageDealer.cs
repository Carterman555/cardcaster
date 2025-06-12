using UnityEngine;

public class DamageDealer {

    public static bool TryDealDamage(GameObject target, Vector2 attackerPos, float damage, float knockbackStrength, bool canCrit = false) {
        bool dealtDamage = false;

        // apply knockback before dealing damage, because dealing damage makes player invincible which makes them
        // not get knockbacked
        if (target.TryGetComponent(out Knockback knockback)) {
            Vector2 toEnemyDirection = (Vector2)target.transform.position - attackerPos;
            knockback.ApplyKnockback(toEnemyDirection, knockbackStrength);
        }
        if (target.TryGetComponent(out IDamagable damagable)) {

            bool crit = canCrit && StatsManager.PlayerStats.CritChance > Random.Range(0f, 1f);
            if (crit) {
                damage *= StatsManager.PlayerStats.CritDamageMult;
            }

            damagable.Damage(damage, crit: crit);
            dealtDamage = true;
        }
        return dealtDamage;
    }

    public static Collider2D[] DealCircleDamage(LayerMask targetLayerMask, Vector2 attackerPos, Vector2 attackCenter, float attackRadius, float damage, float knockbackStrength, bool canCrit = false) {
        Collider2D[] cols = Physics2D.OverlapCircleAll(attackCenter, attackRadius, targetLayerMask);
        foreach (Collider2D col in cols) {
            TryDealDamage(col.gameObject, attackerPos, damage, knockbackStrength, canCrit);
        }
        return cols;
    }

    public static Collider2D[] DealCapsuleDamage(LayerMask targetLayerMask, Vector2 attackerPos, Vector2 attackCenter, Vector2 size, float angle, float damage, float knockbackStrength, bool canCrit = false, bool visualize = false) {
        Collider2D[] cols = Physics2D.OverlapCapsuleAll(attackCenter, size, CapsuleDirection2D.Horizontal, angle, targetLayerMask);
        foreach (Collider2D col in cols) {
            TryDealDamage(col.gameObject, attackerPos, damage, knockbackStrength, canCrit);
        }

        if (visualize) {
            ShapeVisualizer.DrawCapsule(attackCenter, size, CapsuleDirection2D.Horizontal, angle, duration: 0.5f);
        }

        return cols;
    }
}
