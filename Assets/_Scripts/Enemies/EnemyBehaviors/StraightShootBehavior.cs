using System;
using UnityEngine;

public class StraightShootBehavior : EnemyBehavior {
    public event Action OnShootAnim;
    public event Action<Vector2> OnShoot_Direction;

    protected StraightMovement projectilePrefab;
    protected Transform shootPoint;
    protected Transform target;
    private TimedActionBehavior timedActionBehavior;

    public StraightShootBehavior(Enemy enemy, StraightMovement projectilePrefab, Transform shootPoint) : base(enemy) {
        this.projectilePrefab = projectilePrefab;
        this.shootPoint = shootPoint;

        timedActionBehavior = new TimedActionBehavior(
            enemy.GetStats().AttackCooldown,
            () => ShootAnimation()
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

        timedActionBehavior.SetActionCooldown(enemy.GetStats().AttackCooldown);
    }

    private void ShootAnimation() {
        enemy.InvokeAttack();
        OnShootAnim?.Invoke();
    }

    protected virtual void CreateProjectile() {
        StraightMovement newProjectile = projectilePrefab.Spawn(shootPoint.position, Containers.Instance.Projectiles);

        Vector2 toTarget = target.position - enemy.transform.position;
        newProjectile.Setup(toTarget.normalized);
        newProjectile.GetComponent<DamageOnContact>().Setup(enemy.GetStats().Damage, enemy.GetStats().KnockbackStrength);

        InvokeShoot(toTarget.normalized);
    }

    protected void InvokeShoot(Vector2 direction) {
        OnShoot_Direction?.Invoke(direction);
    }

    public override void DoAnimationTriggerEventLogic(AnimationTriggerType triggerType) {
        base.DoAnimationTriggerEventLogic(triggerType);

        if (triggerType == AnimationTriggerType.ShootStraight) {
            CreateProjectile();
        }
    }
}