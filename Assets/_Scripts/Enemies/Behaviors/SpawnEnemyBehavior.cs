using System;
using UnityEngine;

public class SpawnEnemyBehavior : EnemyBehavior {

    public event Action OnSpawnEnemy;

    private Enemy enemyToSpawn;
    private Vector2 localSpawnPosition;

    private float spawnTimer;

    private int amountLeftToSpawn;

    public void Setup(Enemy enemyToSpawn, Vector2 localSpawnPosition) {
        this.enemyToSpawn = enemyToSpawn;
        this.localSpawnPosition = localSpawnPosition;
    }

    public void StartSpawning(int amountToSpawn) {
        Start();
        amountLeftToSpawn = amountToSpawn;
        spawnTimer = 0;
    }

    public override void Stop() {
        base.Stop();
        amountLeftToSpawn = 0;
        spawnTimer = 0;
    }

    public override void FrameUpdateLogic() {

        if (amountLeftToSpawn <= 0) {
            Stop();
        }

        if (!IsStopped()) {
            spawnTimer += Time.deltaTime;
            if (spawnTimer > enemy.GetStats().AttackCooldown) {
                SpawnEnemy();
                spawnTimer = 0;
            }
        }
    }

    private void SpawnEnemy() {
        Vector2 spawnPosition = (Vector2)enemy.transform.position + localSpawnPosition;
        Enemy spawnedEnemy = enemyToSpawn.Spawn(spawnPosition, Containers.Instance.Enemies);

        amountLeftToSpawn--;

        OnSpawnEnemy?.Invoke();
    }
}
