using System;
using UnityEngine;

public class ShootTargetProjectileBehavior : EnemyBehavior {
    public event Action OnShoot;

    private ITargetMovement projectilePrefab;
    private Transform shootPoint;
    private TimedActionBehavior timedActionBehavior;

    public ShootTargetProjectileBehavior(Enemy enemy, ITargetMovement projectilePrefab, Transform localShootPosition) : base(enemy) {
        this.projectilePrefab = projectilePrefab;
        this.shootPoint = localShootPosition;

        timedActionBehavior = new TimedActionBehavior(
            enemy.GetStats().AttackCooldown,
            () => enemy.InvokeAttack()
        );

        Stop();
    }

    public void StartShooting(int amountToShoot) {
        Start();
        timedActionBehavior.Start(amountToShoot);
    }

    public override void Stop() {
        base.Stop();
        timedActionBehavior.Stop();
    }

    public override void FrameUpdateLogic() {
        if (!IsStopped()) {
            timedActionBehavior.UpdateLogic();
            if (timedActionBehavior.IsFinished()) {
                Stop();
            }
        }
    }

    private void ShootProjectile() {
        GameObject newProjectileObject = projectilePrefab.GetObject().Spawn(shootPoint.position, Containers.Instance.Enemies);
        ITargetMovement newProjectile = newProjectileObject.GetComponent<ITargetMovement>();
        newProjectile.Setup(PlayerMovement.Instance.transform);
        newProjectileObject.GetComponent<DamageOnContact>().Setup(enemy.GetStats().Damage, enemy.GetStats().KnockbackStrength);

        OnShoot?.Invoke();
    }

    public override void DoAnimationTriggerEventLogic(AnimationTriggerType triggerType) {
        base.DoAnimationTriggerEventLogic(triggerType);

        if (triggerType == AnimationTriggerType.ShootTargetProjectile) {
            ShootProjectile();
        }
    }
}