using UnityEngine;

public class CursedWitch : Enemy {

    [Header("Movement")]
    [SerializeField] private float moveFromPlayerRange = 3f;
    private FleePlayerBehavior fleeBehavior;

    [Header("Attacks")]
    [SerializeField] private RandomFloat betweenActionDuration;
    private float betweenActionTimer;

    [SerializeField][Range(0f, 1f)] private float spawnEnemiesChance;
    private ShootTargetProjectileBehavior shootProjectileBehavior;
    private SpawnEnemyBehavior spawnEnemyBehavior;

    protected override void Awake() {
        base.Awake();

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

    private void HandleMovement() {

        float distanceFromPlayer = Vector2.Distance(PlayerMovement.Instance.CenterPos, transform.position);

        bool closeToPlayer = distanceFromPlayer < moveFromPlayerRange;

        if (closeToPlayer) {
            if (!fleeBehavior.enabled) {
                fleeBehavior.enabled = true;
            }
        }
        else if (!closeToPlayer) {
            if (fleeBehavior.enabled) {
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