using System;
using UnityEngine;

public class StraightShootBehavior : EnemyBehavior {
    public event Action OnShoot;
    public event Action<Vector2> OnShoot_Direction;

    private StraightMovement projectilePrefab;
    private Vector2 localShootPosition;
    protected Transform target;
    private TimedActionBehavior timedActionBehavior;

    public StraightShootBehavior(Enemy enemy, StraightMovement projectilePrefab, Vector2 localShootPosition) : base(enemy) {
        this.projectilePrefab = projectilePrefab;
        this.localShootPosition = localShootPosition;

        timedActionBehavior = new TimedActionBehavior(
            enemy.GetStats().AttackCooldown,
            () => enemy.InvokeAttack()
        );

        Stop();
    }

    public override void FrameUpdateLogic() {
        base.FrameUpdateLogic();

        if (!IsStopped()) {
            timedActionBehavior.UpdateLogic();
        }
    }

    public void StartShooting(Transform target) {
        Start();
        this.target = target;
        timedActionBehavior.Start();
        // less delay the first time shooting
        timedActionBehavior.SetActionCooldown(enemy.GetStats().AttackCooldown / 2f);
    }

    protected virtual void Shoot() {
        Vector2 shootPosition = (Vector2)enemy.transform.position + localShootPosition;
        StraightMovement newProjectile = projectilePrefab.Spawn(shootPosition, Containers.Instance.Projectiles);

        Vector2 toTarget = target.position - enemy.transform.position;
        newProjectile.Setup(toTarget.normalized);
        newProjectile.GetComponent<DamageOnContact>().Setup(enemy.GetStats().Damage, enemy.GetStats().KnockbackStrength);

        InvokeShoot(toTarget.normalized);
    }

    protected void InvokeShoot(Vector2 direction) {
        OnShoot?.Invoke();
        OnShoot_Direction?.Invoke(direction);
    }

    public override void DoAnimationTriggerEventLogic(AnimationTriggerType triggerType) {
        base.DoAnimationTriggerEventLogic(triggerType);

        if (triggerType == AnimationTriggerType.ShootStraight) {
            Shoot();
        }
    }
}