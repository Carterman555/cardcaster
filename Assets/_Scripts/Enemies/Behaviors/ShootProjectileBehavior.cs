using System;
using UnityEngine;

public class ShootProjectileBehavior : EnemyBehavior {

    public event Action OnShoot;

    private Enemy enemyToSpawn;
    private Vector2 localSpawnPosition;

    private float shootCooldown;
    private float shootTimer;

    private int amountLeftToShoot;

    public void Setup(Enemy enemyToSpawn, Vector2 localSpawnPosition, float shootCooldown) {
        this.enemyToSpawn = enemyToSpawn;
        this.localSpawnPosition = localSpawnPosition;
        this.shootCooldown = shootCooldown;
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
        Vector2 spawnPosition = (Vector2)enemy.transform.position + localSpawnPosition;
        Enemy spawnedEnemy = enemyToSpawn.Spawn(spawnPosition, Containers.Instance.Enemies);

        amountLeftToShoot--;

        OnShoot?.Invoke();
    }

    public bool IsShooting() {
        return amountLeftToShoot > 0;
    }

}
