using UnityEngine;

public class SlashAttackBehavior : UnitBehavior {

    private SlashingWeapon weapon;
    private LayerMask targetLayerMask;

    public void Setup(SlashingWeapon weapon, LayerMask targetLayerMask) {
        this.weapon = weapon;
        this.targetLayerMask = targetLayerMask;
    }

    public void Swing(Vector2 attackDirection, float slashSize) {

        weapon.Swing();

        // deal damage
        Vector2 attackCenter = (Vector2)gameObject.transform.position + (attackDirection.normalized * slashSize);

        Collider2D[] cols = Physics2D.OverlapCircleAll(attackCenter, slashSize, targetLayerMask);
        foreach (Collider2D col in cols) {
            if (col.TryGetComponent(out IDamagable damagable)) {
                damagable.Damage(hasStats.GetStats().Damage);
            }
            if (col.TryGetComponent(out Knockback knockback)) {
                Vector2 toEnemyDirection = col.transform.position - gameObject.transform.position;
                knockback.ApplyKnockback(toEnemyDirection, hasStats.GetStats().KnockbackStrength);
            }
        }
    }
}
