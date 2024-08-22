using System;
using UnityEngine;

public class ShootTargetProjectileBehavior : EnemyBehavior {

    public event Action OnShoot;

    private ITargetProjectile projectile;
    private Vector2 localShootPosition;

    private float shootCooldown;
    private float shootTimer;

    private float damage;

    private int amountLeftToShoot;

    public void Setup(ITargetProjectile projectile, Vector2 localShootPosition, float shootCooldown, float damage) {
        this.projectile = projectile;
        this.localShootPosition = localShootPosition;
        this.shootCooldown = shootCooldown;
        this.damage = damage;
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
            if (shootTimer > shootCooldown) {
                ShootProjectile();
                shootTimer = 0;
            }
        }
    }

    private void ShootProjectile() {
        Vector2 shootPosition = (Vector2)enemy.transform.position + localShootPosition;
        ITargetProjectile newProjectile = projectile.GetObject().Spawn(shootPosition, Containers.Instance.Enemies).GetComponent<ITargetProjectile>();
        newProjectile.Shoot(PlayerMovement.Instance.transform, damage);

        Debug.Log("shoot projectile");

        amountLeftToShoot--;

        OnShoot?.Invoke();
    }

    public bool IsShooting() {
        return amountLeftToShoot > 0;
    }

}
