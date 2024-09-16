using System;
using UnityEngine;

public class SpawnEnemyBehavior : EnemyBehavior {

    public event Action OnSpawnEnemy;

    private Enemy enemyToSpawn;
    private Transform spawnPoint;

    private float spawnTimer;

    private int amountLeftToSpawn;

    // because one anim can be responible for multiple triggers (cursed witch could either be attacking
    // so spawning enemy
    private bool waitingForAnim;

    public void Setup(Enemy enemyToSpawn, Transform spawnPoint) {
        this.enemyToSpawn = enemyToSpawn;
        this.spawnPoint = spawnPoint;
        waitingForAnim = false;
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
                enemy.InvokeAttack();
                spawnTimer = 0;
                waitingForAnim = true;
            }
        }
    }

    private void SpawnEnemy() {
        Enemy spawnedEnemy = enemyToSpawn.Spawn(spawnPoint.position, Containers.Instance.Enemies);

        amountLeftToSpawn--;
        waitingForAnim = false;

        OnSpawnEnemy?.Invoke();
    }

    public override void DoAnimationTriggerEventLogic(AnimationTriggerType triggerType) {
        base.DoAnimationTriggerEventLogic(triggerType);

        if (triggerType == AnimationTriggerType.SpawnEnemy && waitingForAnim) {
            SpawnEnemy();
        }
        else if (triggerType == AnimationTriggerType.Die || triggerType == AnimationTriggerType.Damaged) {
            waitingForAnim = false;
        }
    }
}
