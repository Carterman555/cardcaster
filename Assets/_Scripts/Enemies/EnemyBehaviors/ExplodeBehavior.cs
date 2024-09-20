using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeBehavior : EnemyBehavior {

    public ExplodeBehavior(Enemy enemy) : base(enemy) {
    }

    public void Explode(LayerMask targetLayerMask, float explosionRadius) {

        Collider2D[] cols = Physics2D.OverlapCircleAll(enemy.transform.position, explosionRadius, targetLayerMask);

        foreach (Collider2D col in cols) {
            DamageDealer.TryDealDamage(col.gameObject,
                enemy.transform.position,
                enemy.GetStats().Damage,
                enemy.GetStats().KnockbackStrength);
        }

        enemy.InvokeAttack();
    }
}
