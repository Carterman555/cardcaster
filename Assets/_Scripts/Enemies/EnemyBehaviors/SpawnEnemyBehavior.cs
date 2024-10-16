using System;
using UnityEngine;

public class SpawnEnemyBehavior : MonoBehaviour {

    [SerializeField] private bool specialAttack = true;

    [SerializeField] private Enemy enemyToSpawn;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Animator anim;

    private IHasStats hasStats;
    private TimedActionBehavior timedActionBehavior;

    private void Awake() {

        hasStats = GetComponent<IHasStats>();

        timedActionBehavior = new TimedActionBehavior(
            hasStats.GetStats().AttackCooldown,
            () => TriggerSpawnAnimation()
        );
    }

    public void StartSpawning(int amountToSpawn) {
        timedActionBehavior.Start(amountToSpawn);
    }

    private void OnDisable() {
        timedActionBehavior.Stop();
    }

    private void Update() {
        timedActionBehavior.UpdateLogic();
        if (timedActionBehavior.IsFinished()) {
            enabled = true;
        }
    }

    private void TriggerSpawnAnimation() {
        //... this animation plays SpawnEnemy()
        string attackTriggerString = specialAttack ? "specialAttack" : "attack";
        anim.SetTrigger(attackTriggerString);
    }

    // played by animation
    private void SpawnEnemy() {
        Enemy spawnedEnemy = enemyToSpawn.Spawn(spawnPoint.position, Containers.Instance.Enemies);
    }
}