using System;
using UnityEngine;

public class SpawnEnemyBehavior : EnemyBehavior {

    public event Action OnSpawnEnemy;

    private Enemy enemyToSpawn;
    private Transform spawnPoint;

    private float spawnTimer;

    private int amountLeftToSpawn;

    public void Setup(Enemy enemyToSpawn, Transform spawnPoint) {
        this.enemyToSpawn = enemyToSpawn;
        this.spawnPoint = spawnPoint;
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
                enemy.InvokeSpecialAttack();
                spawnTimer = 0;
            }
        }
    }

    private void SpawnEnemy() {
        Enemy spawnedEnemy = enemyToSpawn.Spawn(spawnPoint.position, Containers.Instance.Enemies);

        amountLeftToSpawn--;

        OnSpawnEnemy?.Invoke();
    }

    public override void DoAnimationTriggerEventLogic(AnimationTriggerType triggerType) {
        base.DoAnimationTriggerEventLogic(triggerType);

        if (triggerType == AnimationTriggerType.SpawnEnemy) {
            SpawnEnemy();
        }
    }
}
