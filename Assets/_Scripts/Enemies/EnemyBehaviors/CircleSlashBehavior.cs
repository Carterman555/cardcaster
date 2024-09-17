using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleSlashBehavior : EnemyBehavior {

    private Transform centerPoint;

    private float attackTimer;

    public CircleSlashBehavior(Enemy enemy, Transform centerPoint) : base(enemy) {
        this.centerPoint = centerPoint;
    }

    public override void FrameUpdateLogic() {
        base.FrameUpdateLogic();

        if (IsStopped()) {
            attackTimer = 0f;
            return;
        }

        attackTimer += Time.deltaTime;
        if (attackTimer > enemy.GetStats().AttackCooldown) {
            enemy.InvokeAttack();
            attackTimer = 0f;
        }
    }

    private void Attack() {
        CircleDamage.DealDamage(GameLayers.PlayerLayerMask,
            centerPoint.position,
            enemy.GetEnemyStats().AttackRange,
            enemy.GetStats().Damage,
            enemy.GetStats().KnockbackStrength);
    }

    public override void DoAnimationTriggerEventLogic(AnimationTriggerType triggerType) {
        base.DoAnimationTriggerEventLogic(triggerType);

        if (triggerType == AnimationTriggerType.CircleSlash) {
            Attack();
        }
    }
}
