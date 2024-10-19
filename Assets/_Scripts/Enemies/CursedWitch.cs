using UnityEngine;
using UnityEngine.AI;

public class CursedWitch : Enemy {

    [Header("Movement")]
    [SerializeField] private float chasePlayerRange = 4f;
    [SerializeField] private float moveFromPlayerRange = 3f;
    private ChasePlayerBehavior chaseBehavior;
    private FleePlayerBehavior fleeBehavior;

    [Header("Attacks")]
    [SerializeField] private RandomFloat betweenActionDuration;
    private float betweenActionTimer;

    [SerializeField][Range(0f, 1f)] private float spawnEnemiesChance;
    private ShootTargetProjectileBehavior shootProjectileBehavior;
    private SpawnEnemyBehavior spawnEnemyBehavior;

    protected override void Awake() {
        base.Awake();

        chaseBehavior = GetComponent<ChasePlayerBehavior>();
        fleeBehavior = GetComponent<FleePlayerBehavior>();
        shootProjectileBehavior = GetComponent<ShootTargetProjectileBehavior>();
        spawnEnemyBehavior = GetComponent<SpawnEnemyBehavior>();
    }

    protected override void OnEnable() {
        base.OnEnable();
        betweenActionDuration.Randomize();

        shootProjectileBehavior.enabled = false;
        spawnEnemyBehavior.enabled = false;
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

        if (farFromPlayer) {
            if (!chaseBehavior.enabled || fleeBehavior.enabled) {
                fleeBehavior.enabled = false;
                chaseBehavior.enabled = true;
            }
        }
        else if (closeToPlayer) {
            if (chaseBehavior.enabled || !fleeBehavior.enabled) {
                chaseBehavior.enabled = false;
                fleeBehavior.enabled = true;
            }
        }
        else if (!farFromPlayer && !closeToPlayer) {
            if (chaseBehavior.enabled || fleeBehavior.enabled) {
                chaseBehavior.enabled = false;
                fleeBehavior.enabled = false;
            }
        }
    }

    private bool PerformingAction => shootProjectileBehavior.enabled || spawnEnemyBehavior.enabled;

    private void HandleAction() {
        if (!PerformingAction) {
            betweenActionTimer += Time.deltaTime;
            if (betweenActionTimer > betweenActionDuration.Value) {
                betweenActionTimer = 0;
                betweenActionDuration.Randomize();

                if (spawnEnemiesChance > Random.value) {
                    spawnEnemyBehavior.enabled = true;
                }
                else {
                    shootProjectileBehavior.enabled = true;
                }
            }
        }
        else {
            betweenActionTimer = 0;
        }
    }
}