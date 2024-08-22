using UnityEngine;

public class CursedWitch : Enemy {

    [Header("General")]
    [SerializeField] private RandomFloat betweenActionDuration;
    private float betweenActionTimer;

    [SerializeField][Range(0f, 1f)] private float spawnEnemiesChance;

    [SerializeField] private Transform spawnPoint;

    [Header("Movement")]
    [SerializeField] private float chasePlayerRange = 4f;
    private ChasePlayerBehavior chasePlayerBehavior;

    [SerializeField] private float moveFromPlayerRange = 3f;
    private MoveFromPlayerBehavior moveFromPlayerBehavior;

    private MoveType currentMoveType;

    [Header("Shoot Projectile")]
    [SerializeField] private RandomInt projectileShootAmount;
    [SerializeField] private HeatSeekProjectile projectile;
    private ShootTargetProjectileBehavior shootProjectileBehavior;


    [Header("Spawn Enemy")]
    [SerializeField] private RandomInt enemySpawnAmount;
    [SerializeField] private Enemy enemyToSpawn;
    private SpawnEnemyBehavior spawnEnemyBehavior;


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
        shootProjectileBehavior.Setup(projectile, spawnPoint.position, stats.AttackCooldown, stats.Damage);
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

        HandleMovement();

        HandleAction();
    }

    /// <summary>
    /// if far from player, chase
    /// if close, move away
    /// if midrange, stop
    /// </summary>
    private void HandleMovement() {

        float distanceFromPlayer = Vector2.Distance(PlayerMovement.Instance.transform.position, transform.position);

        bool farFromPlayer = distanceFromPlayer > chasePlayerRange;
        bool closeToPlayer = distanceFromPlayer < moveFromPlayerRange;

        if (farFromPlayer && currentMoveType != MoveType.Chase) {
            chasePlayerBehavior.Start();
            moveFromPlayerBehavior.Stop();
            currentMoveType = MoveType.Chase;
        }
        else if (closeToPlayer && currentMoveType != MoveType.Run) {
            chasePlayerBehavior.Stop();
            moveFromPlayerBehavior.Start();
            currentMoveType = MoveType.Run;
        }
        else if (!farFromPlayer && !closeToPlayer && currentMoveType != MoveType.Stationary) {
            chasePlayerBehavior.Stop();
            moveFromPlayerBehavior.Stop();
            currentMoveType = MoveType.Stationary;
        }
    }

    private void HandleAction() {
        bool performingAction = spawnEnemyBehavior.IsSpawningEnemies();
        if (!performingAction) {
            betweenActionTimer += Time.deltaTime;
            if (betweenActionTimer > betweenActionDuration.Value) {
                betweenActionTimer = 0;
                betweenActionDuration.Randomize();

                print("start action");

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
    }
}

public enum MoveType { Chase, Run, Stationary }
