using System;
using UnityEngine;

public class ShootTargetProjectileBehavior : EnemyBehavior {

    public event Action OnShoot;

    private ITargetMovement projectilePrefab;
    private Transform shootPoint;

    private float shootTimer;
    private int amountLeftToShoot;

    public ShootTargetProjectileBehavior(Enemy enemy, ITargetMovement projectilePrefab, Transform localShootPosition) : base(enemy) {
        this.projectilePrefab = projectilePrefab;
        this.shootPoint = localShootPosition;

        Stop();
    }
        
    public void StartShooting(int amountToShoot) {
        Start();
        amountLeftToShoot = amountToShoot;
        shootTimer = 0;
    }

    public override void Stop() {
        base.Stop();
        amountLeftToShoot = 0;
        shootTimer = 0;
    }

    public override void FrameUpdateLogic() {

        if (amountLeftToShoot <= 0) {
            Stop();
        }

        if (!IsStopped()) {
            shootTimer += Time.deltaTime;
            if (shootTimer > enemy.GetStats().AttackCooldown) {
                enemy.InvokeAttack();
                shootTimer = 0;
            }
        }
    }

    private void ShootProjectile() {
        GameObject newProjectileObject = projectilePrefab.GetObject().Spawn(shootPoint.position, Containers.Instance.Enemies);
        ITargetMovement newProjectile = newProjectileObject.GetComponent<ITargetMovement>();
        newProjectile.Setup(PlayerMovement.Instance.transform);
        newProjectileObject.GetComponent<DamageOnContact>().Setup(enemy.GetStats().Damage, enemy.GetStats().KnockbackStrength);

        amountLeftToShoot--;

        OnShoot?.Invoke();
    }

    public override void DoAnimationTriggerEventLogic(AnimationTriggerType triggerType) {
        base.DoAnimationTriggerEventLogic(triggerType);

        if (triggerType == AnimationTriggerType.ShootTargetProjectile) {
            ShootProjectile();
        }
    }
}
