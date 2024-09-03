using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleSlashBehavior : EnemyBehavior {

    private LayerMask playerLayer;

    private float attackTimer;

    public void Setup(LayerMask playerLayer) {
        this.playerLayer = playerLayer;
    }

    public override void FrameUpdateLogic() {
        base.FrameUpdateLogic();

        if (IsStopped()) {
            attackTimer = 0f;
            return;
        }

        attackTimer += Time.deltaTime;
        if (attackTimer > enemy.GetStats().AttackCooldown) {
            Attack();
            attackTimer = 0f;
        }
    }

    private void Attack() {
        CircleDamage.DealDamage(playerLayer,
            enemy.transform.position,
            enemy.GetEnemyStats().AttackRange,
            enemy.GetStats().Damage,
            enemy.GetStats().KnockbackStrength);
    }
}
