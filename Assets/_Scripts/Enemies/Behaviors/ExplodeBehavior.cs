using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeBehavior : EnemyBehavior {

    public void Explode(LayerMask targetLayerMask, float explosionRadius, float damage) {

        Collider2D[] cols = Physics2D.OverlapCircleAll(enemy.transform.position, explosionRadius, targetLayerMask);

        foreach (Collider2D col in cols) {
            if (col.TryGetComponent(out Health health)) {
                health.Damage(damage);
            }
            //if (col.TryGetComponent(out Knockback knockback)) {

            //}
        }

    }

}
