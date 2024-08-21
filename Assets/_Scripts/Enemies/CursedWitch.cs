using UnityEngine;

public class CursedWitch : Enemy {

    [Header("General")]
    [SerializeField] private RandomFloat betweenActionDuration;
    private float betweenActionTimer;

    [SerializeField][Range(0f, 1f)] private float spawnEnemiesChance;

    [SerializeField] private Transform spawnPoint;


    private ChasePlayerBehavior chasePlayerBehavior;
    private MoveFromPlayerBehavior moveFromPlayerBehavior;

    [Header("Shoot Projectile")]
    private ShootProjectileBehavior shootProjectileBehavior;
    [SerializeField] private RandomInt projectileShootAmount;

    [Header("Spawn Enemy")]
    private SpawnEnemyBehavior spawnEnemyBehavior;
    [SerializeField] private RandomInt enemySpawnAmount;
    [SerializeField] private Enemy enemyToSpawn;
    

    private void OnEnable() {
        InitializeBehaviors();

        betweenActionDuration.Randomize();
    }

    private void InitializeBehaviors() {
        chasePlayerBehavior = new();
        chasePlayerBehavior.SetSpeed(stats.MoveSpeed);
        enemyBehaviors.Add(chasePlayerBehavior);

        moveFromPlayerBehavior = new();
        moveFromPlayerBehavior.SetSpeed(stats.MoveSpeed);
        enemyBehaviors.Add(moveFromPlayerBehavior);

        shootProjectileBehavior = new();
        //shootProjectileBehavior.Setup(stats.MoveSpeed);
        enemyBehaviors.Add(shootProjectileBehavior);

        spawnEnemyBehavior = new();
        spawnEnemyBehavior.Setup(enemyToSpawn, spawnPoint.position, stats.AttackCooldown);
        enemyBehaviors.Add(spawnEnemyBehavior);

        foreach (var enemyBehavior in enemyBehaviors) {
            enemyBehavior.Initialize(this);
        }
    }

    protected override void Update() {
        base.Update();

        bool performingAction = spawnEnemyBehavior.IsSpawningEnemies();
        if (!performingAction) {
            betweenActionTimer += Time.deltaTime;
            if (betweenActionTimer > betweenActionDuration.Value) {
                betweenActionTimer = 0;
                betweenActionDuration.Randomize();

                if (spawnEnemiesChance > Random.value) {
                    spawnEnemyBehavior.StartSpawning(enemySpawnAmount.Randomize());
                }
                else {
                    shootProjectileBehavior.StartShooting(projectileShootAmount.Randomize());
                }
            }
        }
        else {
            betweenActionTimer = 0;
        }

        spawnEnemyBehavior.StartSpawning(enemySpawnAmount.Randomize());
    }
}
