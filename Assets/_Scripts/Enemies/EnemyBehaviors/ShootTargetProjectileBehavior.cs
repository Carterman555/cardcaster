using System;
using UnityEngine;

public class ShootTargetProjectileBehavior : EnemyBehavior {

    public event Action OnShoot;

    private ITargetMovement projectilePrefab;
    private Transform shootPoint;

    private float shootTimer;
    private int amountLeftToShoot;

    // because one anim can be responible for multiple triggers (cursed witch could either be attacking
    // so spawning enemy
    private bool waitingForAnimToShoot; 

    public void Setup(ITargetMovement projectilePrefab, Transform localShootPosition) {
        this.projectilePrefab = projectilePrefab;
        this.shootPoint = localShootPosition;
        waitingForAnimToShoot = false;

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

                waitingForAnimToShoot = true;
            }
        }
    }

    private void ShootProjectile() {
        GameObject newProjectileObject = projectilePrefab.GetObject().Spawn(shootPoint.position, Containers.Instance.Enemies);
        ITargetMovement newProjectile = newProjectileObject.GetComponent<ITargetMovement>();
        newProjectile.Setup(PlayerMovement.Instance.transform);
        newProjectileObject.GetComponent<DamageOnContact>().Setup(enemy.GetStats().Damage, enemy.GetStats().KnockbackStrength);

        amountLeftToShoot--;
        waitingForAnimToShoot = false;

        OnShoot?.Invoke();
    }

    public override void DoAnimationTriggerEventLogic(AnimationTriggerType triggerType) {
        base.DoAnimationTriggerEventLogic(triggerType);

        if (triggerType == AnimationTriggerType.RangedAttack && waitingForAnimToShoot) {
            ShootProjectile();
        }
        else if (triggerType == AnimationTriggerType.Die || triggerType == AnimationTriggerType.Damaged) {
            waitingForAnimToShoot = false;
        }
    }
}
