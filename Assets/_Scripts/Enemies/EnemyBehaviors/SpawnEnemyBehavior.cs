using System;
using UnityEngine;

public class SpawnEnemyBehavior : MonoBehaviour {

    [SerializeField] private bool specialAttack = true;

    [SerializeField] private RandomInt amountToSpawn;
    [SerializeField] private Enemy enemyToSpawn;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Animator anim;

    private IHasEnemyStats hasStats;
    private TimedActionBehavior timedActionBehavior;

    [Header("SFX")]
    [SerializeField] private bool customSFX;
    [ConditionalHide("customSFX")][SerializeField] private AudioClips spawnSFX;

    private void Awake() {

        hasStats = GetComponent<IHasEnemyStats>();

        timedActionBehavior = new TimedActionBehavior(
            hasStats.            EnemyStats.AttackCooldown,
            () => TriggerSpawnAnimation()
        );
    }

    public void OnEnable() {
        timedActionBehavior.Start(amountToSpawn.Randomize());
    }
    private void OnDisable() {
        timedActionBehavior.Stop();
    }

    private void Update() {
        timedActionBehavior.UpdateLogic();
        if (timedActionBehavior.IsFinished()) {
            enabled = false;
        }
    }

    private void TriggerSpawnAnimation() {
        //... this animation plays SpawnEnemy()
        string attackTriggerString = specialAttack ? "specialAttack" : "attack";
        anim.SetTrigger(attackTriggerString);
    }

    // played by animation
    public void SpawnEnemy() {
        Enemy spawnedEnemy = enemyToSpawn.Spawn(spawnPoint.position, Containers.Instance.Enemies);

        if (customSFX) {
            AudioManager.Instance.PlaySound(spawnSFX);
        }
        else {
            AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.SpawnEnemy);
        }
    }
}