using System;
using UnityEngine;

public class ShootTargetProjectileBehavior : EnemyBehavior {

    public event Action OnShoot;

    private ITargetProjectile projectile;
    private Vector2 localShootPosition;

    private float shootTimer;

    private int amountLeftToShoot;

    public void Setup(ITargetProjectile projectile, Vector2 localShootPosition) {
        this.projectile = projectile;
        this.localShootPosition = localShootPosition;
    }

    public void StartShooting(int amountToShoot) {
        amountLeftToShoot = amountToShoot;
        shootTimer = 0;
    }

    public void StopShooting() {
        amountLeftToShoot = 0;
        shootTimer = 0;
    }

    public override void FrameUpdateLogic() {

        if (IsShooting()) {
            shootTimer += Time.deltaTime;
            if (shootTimer > enemy.GetStats().AttackCooldown) {
                ShootProjectile();
                shootTimer = 0;
            }
        }
    }

    private void ShootProjectile() {
        Vector2 shootPosition = (Vector2)enemy.transform.position + localShootPosition;
        ITargetProjectile newProjectile = projectile.GetObject().Spawn(shootPosition, Containers.Instance.Enemies).GetComponent<ITargetProjectile>();
        newProjectile.Shoot(PlayerMovement.Instance.transform, enemy.GetStats().Damage);

        amountLeftToShoot--;

        OnShoot?.Invoke();
    }

    public bool IsShooting() {
        return amountLeftToShoot > 0;
    }
}
