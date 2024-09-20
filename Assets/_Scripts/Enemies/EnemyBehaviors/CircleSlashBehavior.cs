using UnityEngine;

public class CircleSlashBehavior : EnemyBehavior {
    private TimedActionBehavior timedActionBehavior;
    private Transform centerPoint;

    public CircleSlashBehavior(Enemy enemy, Transform centerPoint) : base(enemy) {
        this.centerPoint = centerPoint;

        timedActionBehavior = new TimedActionBehavior(
            enemy.GetStats().AttackCooldown,
            () => enemy.InvokeAttack()
        );

        Stop();
    }

    public override void Start() {
        base.Start();
        timedActionBehavior.Start();
    }

    public override void Stop() {
        base.Stop();
        timedActionBehavior.Stop();
    }

    public override void FrameUpdateLogic() {
        base.FrameUpdateLogic();
        if (!IsStopped()) {
            timedActionBehavior.UpdateLogic();
        }
    }

    private void Attack() {
        DamageDealer.DealCircleDamage(
            GameLayers.PlayerLayerMask,
            centerPoint.position,
            enemy.GetEnemyStats().AttackRange,
            enemy.GetStats().Damage,
            enemy.GetStats().KnockbackStrength
        );
    }

    public override void DoAnimationTriggerEventLogic(AnimationTriggerType triggerType) {
        base.DoAnimationTriggerEventLogic(triggerType);
        if (triggerType == AnimationTriggerType.CircleSlash) {
            Attack();
        }
    }
}