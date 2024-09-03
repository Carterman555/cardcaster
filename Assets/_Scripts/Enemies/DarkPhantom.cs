using UnityEngine;

public class DarkPhantom : Enemy {
    private ChasePlayerBehavior moveBehavior;

    [Header("Attack")]
    private StraightShootBehavior shootBehavior;
    [SerializeField] private BasicProjectile projectile;
    [SerializeField] private Transform shootPoint;

    [Header("Teleport")]
    private RandomTeleportBehavior teleportBehavior;
    [SerializeField] private float teleportDistanceFromPlayer = 3f;
    [SerializeField] private RandomFloat nearPlayerTeleportTime;
    private float nearPlayerTeleportTimer;

    protected override void OnEnable() {
        base.OnEnable();
        InitializeBehaviors();

        moveBehavior.Start();

        nearPlayerTeleportTime.Randomize();
    }

    private void InitializeBehaviors() {
        moveBehavior = new();
        shootBehavior = new();
        enemyBehaviors.Add(moveBehavior);
        enemyBehaviors.Add(shootBehavior);

        foreach (var enemyBehavior in enemyBehaviors) {
            enemyBehavior.Initialize(this);
        }

        shootBehavior.Setup(projectile, shootPoint.localPosition);
        shootBehavior.StartShooting(PlayerMovement.Instance.transform);

        PolygonCollider2D teleportBounds = Room.GetCurrentRoom().GetComponent<PolygonCollider2D>();
        teleportBehavior = new();
        teleportBehavior.Initialize(gameObject, this);
        teleportBehavior.Setup(teleportBounds);
    }

    protected override void Update() {
        base.Update();

        if (playerWithinRange) {
            nearPlayerTeleportTimer += Time.deltaTime;
            if (nearPlayerTeleportTimer > nearPlayerTeleportTime.Value) {
                nearPlayerTeleportTimer = 0;
                nearPlayerTeleportTime.Randomize();

                teleportBehavior.Teleport(PlayerMovement.Instance.transform.position, teleportDistanceFromPlayer);
            }
        }
        else {
            nearPlayerTeleportTimer = 0;
        }
    }
}
