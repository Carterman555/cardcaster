using System;
using UnityEngine;

public class SpawnEnemyBehavior : EnemyBehavior {
    public event Action OnSpawnEnemy;

    private Enemy enemyToSpawn;
    private Transform spawnPoint;
    private TimedActionBehavior timedActionBehavior;

    public SpawnEnemyBehavior(Enemy enemy, Enemy enemyToSpawn, Transform spawnPoint) : base(enemy) {
        this.enemyToSpawn = enemyToSpawn;
        this.spawnPoint = spawnPoint;

        timedActionBehavior = new TimedActionBehavior(
            enemy.GetStats().AttackCooldown,
            () => enemy.InvokeSpecialAttack()
        );
    }

    public void StartSpawning(int amountToSpawn) {
        Start();
        timedActionBehavior.Start(amountToSpawn);
    }

    public override void Stop() {
        base.Stop();
        timedActionBehavior.Stop();
    }

    public override void FrameUpdateLogic() {
        if (!IsStopped()) {
            timedActionBehavior.UpdateLogic();
            if (timedActionBehavior.IsFinished()) {
                Stop();
            }
        }
    }

    private void SpawnEnemy() {
        Enemy spawnedEnemy = enemyToSpawn.Spawn(spawnPoint.position, Containers.Instance.Enemies);
        OnSpawnEnemy?.Invoke();
    }

    public override void DoAnimationTriggerEventLogic(AnimationTriggerType triggerType) {
        base.DoAnimationTriggerEventLogic(triggerType);

        if (triggerType == AnimationTriggerType.SpawnEnemy) {
            SpawnEnemy();
        }
    }
}