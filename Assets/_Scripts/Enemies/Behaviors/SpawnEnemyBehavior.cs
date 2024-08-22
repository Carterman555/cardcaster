using System;
using UnityEngine;

public class SpawnEnemyBehavior : EnemyBehavior {

    public event Action OnSpawnEnemy;

    private Enemy enemyToSpawn;
    private Vector2 localSpawnPosition;

    private float spawnCooldown;
    private float spawnTimer;

    private int amountLeftToSpawn;

    public void Setup(Enemy enemyToSpawn, Vector2 localSpawnPosition, float spawnCooldown) {
        this.enemyToSpawn = enemyToSpawn;
        this.localSpawnPosition = localSpawnPosition;
        this.spawnCooldown = spawnCooldown;
    }

    public void StartSpawning(int amountToSpawn) {
        amountLeftToSpawn = amountToSpawn;
        spawnTimer = 0;
    }

    public void StopSpawning() {
        amountLeftToSpawn = 0;
        spawnTimer = 0;
    }

    public override void FrameUpdateLogic() {

        if (IsSpawningEnemies()) {
            spawnTimer += Time.deltaTime;
            if (spawnTimer > spawnCooldown) {
                SpawnEnemy();
                spawnTimer = 0;
            }
        }
    }

    private void SpawnEnemy() {
        Vector2 spawnPosition = (Vector2)enemy.transform.position + localSpawnPosition;
        Enemy spawnedEnemy = enemyToSpawn.Spawn(spawnPosition, Containers.Instance.Enemies);

        Debug.Log("spawn enemy");

        amountLeftToSpawn--;

        OnSpawnEnemy?.Invoke();
    }

    public bool IsSpawningEnemies() {
        return amountLeftToSpawn > 0;
    }
}
