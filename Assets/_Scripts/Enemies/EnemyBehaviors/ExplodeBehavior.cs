using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeBehavior : EnemyBehavior {

    public ExplodeBehavior(Enemy enemy) : base(enemy) {
    }

    public void Explode(LayerMask targetLayerMask, float explosionRadius) {

        Collider2D[] cols = Physics2D.OverlapCircleAll(enemy.transform.position, explosionRadius, targetLayerMask);

        foreach (Collider2D col in cols) {
            if (col.TryGetComponent(out IDamagable damagable)) {
                damagable.Damage(enemy.GetStats().Damage);
            }
            if (col.TryGetComponent(out Knockback knockback)) {
                Vector2 toColDirection = col.transform.position - enemy.transform.position;
                knockback.ApplyKnockback(toColDirection, enemy.GetStats().KnockbackStrength);
            }
        }

        enemy.InvokeAttack();
    }
}
