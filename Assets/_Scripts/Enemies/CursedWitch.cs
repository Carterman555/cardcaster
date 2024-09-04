using UnityEngine;
using UnityEngine.AI;

public class CursedWitch : Enemy {

    [Header("General")]
    [SerializeField] private RandomFloat betweenActionDuration;
    private float betweenActionTimer;

    [SerializeField][Range(0f, 1f)] private float spawnEnemiesChance;

    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Animator wandAnim;

    [Header("Movement")]
    [SerializeField] private float chasePlayerRange = 4f;
    [SerializeField] private float moveFromPlayerRange = 3f;
    private ChasePlayerBehavior chaseBehavior;
    private FleePlayerBehavior fleeBehavior;

    [Header("Shoot Projectile")]
    [SerializeField] private RandomInt projectileShootAmount;
    [SerializeField] private HeatSeekProjectile projectile;
    private ShootTargetProjectileBehavior shootProjectileBehavior;

    [Header("Spawn Enemy")]
    [SerializeField] private RandomInt enemySpawnAmount;
    [SerializeField] private Enemy enemyToSpawn;
    private SpawnEnemyBehavior spawnEnemyBehavior;

    protected override void OnEnable() {
        base.OnEnable();
        InitializeBehaviors();

        betweenActionDuration.Randomize();

        shootProjectileBehavior.OnShoot += WandShootAnim;
        spawnEnemyBehavior.OnSpawnEnemy += WandShootAnim;
    }

    protected override void OnDisable() {
        base.OnDisable();

        shootProjectileBehavior.OnShoot -= WandShootAnim;
        spawnEnemyBehavior.OnSpawnEnemy -= WandShootAnim;
    }

    private void InitializeBehaviors() {
        chaseBehavior = new();
        enemyBehaviors.Add(chaseBehavior);

        fleeBehavior = new();
        enemyBehaviors.Add(fleeBehavior);

        shootProjectileBehavior = new();
        shootProjectileBehavior.Setup(projectile, spawnPoint.localPosition);
        enemyBehaviors.Add(shootProjectileBehavior);

        spawnEnemyBehavior = new();
        spawnEnemyBehavior.Setup(enemyToSpawn, spawnPoint.localPosition);
        enemyBehaviors.Add(spawnEnemyBehavior);

        foreach (var enemyBehavior in enemyBehaviors) {
            enemyBehavior.Initialize(this);
        }
    }

    protected override void Update() {
        base.Update();

        HandleMovement();

        HandleAction();

        UpdateWandAnim();
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

        if (farFromPlayer) {
            if (chaseBehavior.IsStopped() || !fleeBehavior.IsStopped()) {
                fleeBehavior.Stop();
                chaseBehavior.Start();
            }
        }
        else if (closeToPlayer) {
            if (!chaseBehavior.IsStopped() || fleeBehavior.IsStopped()) {
                chaseBehavior.Stop();
                fleeBehavior.Start();
            }
        }
        else if (!farFromPlayer && !closeToPlayer) {
            if (!chaseBehavior.IsStopped() || !fleeBehavior.IsStopped()) {
                chaseBehavior.Stop();
                fleeBehavior.Stop();
            }
        }
    }

    private bool PerformingAction => !spawnEnemyBehavior.IsStopped() || !shootProjectileBehavior.IsStopped();

    private void HandleAction() {
        if (!PerformingAction) {
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
    }

    [SerializeField] private PointTowardsPlayer wandPoint;
    [SerializeField] private Recoil wandRecoil;

    private void UpdateWandAnim() {
        bool swingArm = !chaseBehavior.IsStopped() && !PerformingAction;
        wandAnim.SetBool("swinging", swingArm);

        wandPoint.enabled = PerformingAction;
    }

    private void WandShootAnim() {
        wandRecoil.RecoilWeapon();
    }
}