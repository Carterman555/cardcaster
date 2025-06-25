using UnityEngine;

public class Wraith : Enemy {

    private enum WraithState { NotAttacking, MovingTowardsPlayer, Attack, MovingFromPlayer };
    private WraithState currentState;

    private FacePlayerBehavior facePlayerBehavior;

    protected override void Awake() {
        base.Awake();

        rb = GetComponent<Rigidbody2D>();
        facePlayerBehavior = GetComponent<FacePlayerBehavior>();
    }

    protected override void OnEnable() {
        base.OnEnable();

        noiseXOffset = Random.Range(-100f, 100f);
        noiseYOffset = Random.Range(-100f, 100f);
        noiseTime = Random.Range(-100f, 100f);

        currentState = WraithState.NotAttacking;
        attackTimer = 0;
    }

    protected override void Update() {
        base.Update();

        noiseTime += scrollSpeed * Time.deltaTime;

        if (currentState == WraithState.NotAttacking) {
            HandleMovement();

            if (playerWithinRange) {
                attackTimer += Time.deltaTime;
            }
            else {
                attackTimer = 0;
            }

            if (attackTimer > GetEnemyStats().AttackCooldown) {
                currentState = WraithState.MovingTowardsPlayer;
                attackTimer = 0;
            }
        }
        else {
            HandleAttack();
        }

        rb.velocity = Vector2.MoveTowards(rb.velocity, desiredVelocity, moveAcceleration * Time.deltaTime);
    }

    #region Movement

    [Header("Movement")]
    [SerializeField] private float targetPlayerDistance;
    [SerializeField] private float maxTargetDistanceVariation = 3f;
    [SerializeField] private float scrollSpeed = 0.5f;
    [SerializeField] private float moveAcceleration;

    private Vector2 desiredVelocity;

    private Rigidbody2D rb;

    private float noiseXOffset;
    private float noiseYOffset;
    private float noiseTime;

    private Vector2 GetNoiseDirection() {
        float noiseX = Mathf.PerlinNoise(noiseTime + noiseXOffset, 0f);
        float noiseY = Mathf.PerlinNoise(0f, noiseTime + noiseYOffset);

        // Map to -1 to 1
        float vx = (noiseX - 0.5f) * 2f;
        float vz = (noiseY - 0.5f) * 2f;

        Vector2 noiseDirection = new Vector2(vx, vz);
        return noiseDirection;
    }

    private void HandleMovement() {

        Vector2 noiseDirection = GetNoiseDirection();

        // calculate player direction
        float playerDistance = Vector2.Distance(PlayerMovement.Instance.CenterPos, transform.position);
        bool farFromPlayer = playerDistance > targetPlayerDistance;

        Vector2 toPlayerDirection = (PlayerMovement.Instance.CenterPos - transform.position).normalized;
        Vector2 fromPlayerDirection = (transform.position - PlayerMovement.Instance.CenterPos).normalized;
        Vector2 playerDirection = farFromPlayer ? toPlayerDirection : fromPlayerDirection;


        // combined direction
        float distanceVariation = Mathf.Abs(targetPlayerDistance - playerDistance);
        float distanceVariationSqr = distanceVariation * playerDistance;
        float maxTargetDistanceVariationSqr = maxTargetDistanceVariation * maxTargetDistanceVariation;

        float playerWeight = Mathf.InverseLerp(0f, maxTargetDistanceVariationSqr, distanceVariationSqr);
        float noiseWeight = 1 - playerWeight;

        Vector2 direction = (noiseDirection * noiseWeight) + (playerDirection * playerWeight);
        desiredVelocity = direction * GetEnemyStats().MoveSpeed;
    }

    #endregion


    #region Attack

    [Header("Attack")]
    [SerializeField] private float attackMoveSpeed;
    [SerializeField] private float attackDistance;

    [SerializeField] private WraithAttack attackPrefab;
    [SerializeField] private Transform attackPoint;

    private float attackTimer;

    private void HandleAttack() {

        if (currentState == WraithState.MovingTowardsPlayer) {
            Vector2 toPlayerDirection = (PlayerMovement.Instance.CenterPos - transform.position).normalized;
            desiredVelocity = toPlayerDirection * attackMoveSpeed;

            float distanceFromPlayer = Vector2.Distance(transform.position, PlayerMovement.Instance.CenterPos);
            if (distanceFromPlayer < attackDistance) {
                currentState = WraithState.Attack;
                anim.SetTrigger("attack");
            }
        }
        else if (currentState == WraithState.Attack) {
            // move with noise
            desiredVelocity = GetNoiseDirection() * GetEnemyStats().MoveSpeed;
        }
        else if (currentState == WraithState.MovingFromPlayer) {
            Vector2 fromPlayerDirection = (transform.position - PlayerMovement.Instance.CenterPos).normalized;
            desiredVelocity = fromPlayerDirection * attackMoveSpeed;

            float distanceFromPlayer = Vector2.Distance(transform.position, PlayerMovement.Instance.CenterPos);
            if (distanceFromPlayer > targetPlayerDistance) {
                currentState = WraithState.NotAttacking;
            }
        }
    }

    // played by anim
    public void Attack() {
        WraithAttack attack = attackPrefab.Spawn(attackPoint.position, Containers.Instance.Projectiles);

        Vector2 direction = facePlayerBehavior.FacingRight ? Vector2.right : Vector2.left;
        attack.Setup(transform, GetEnemyStats().Damage, GetEnemyStats().KnockbackStrength, direction);

        currentState = WraithState.MovingFromPlayer;
    }

    #endregion
}
