using System;
using UnityEngine;

public class ShootTargetProjectileBehavior : EnemyBehavior {

    public event Action OnShoot;

    private ITargetMovement projectilePrefab;
    private Vector2 localShootPosition;

    private float shootTimer;

    private int amountLeftToShoot;

    public void Setup(ITargetMovement projectilePrefab, Vector2 localShootPosition) {
        this.projectilePrefab = projectilePrefab;
        this.localShootPosition = localShootPosition;

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
                ShootProjectile();
                shootTimer = 0;
            }
        }
    }

    private void ShootProjectile() {
        Vector2 shootPosition = (Vector2)enemy.transform.position + localShootPosition;
        GameObject newProjectileObject = projectilePrefab.GetObject().Spawn(shootPosition, Containers.Instance.Enemies);
        ITargetMovement newProjectile = newProjectileObject.GetComponent<ITargetMovement>();
        newProjectile.Setup(PlayerMovement.Instance.transform);
        newProjectileObject.GetComponent<DamageOnContact>().Setup(enemy.GetStats().Damage, enemy.GetStats().KnockbackStrength);

        amountLeftToShoot--;

        OnShoot?.Invoke();
    }
}
