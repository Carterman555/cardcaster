using UnityEngine;

public class Scattershot : Enemy {

    private WanderMovementBehavior wanderMovement;
    private ShootStraightSpreadBehavior shootBehavior;

    [Header("Recoil")]
    [SerializeField] private Recoil recoil;
    [SerializeField] private float recoilForce;
    private Rigidbody2D rb;

    protected override void Awake() {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();

        wanderMovement = GetComponent<WanderMovementBehavior>();
        shootBehavior = GetComponent<ShootStraightSpreadBehavior>();
    }

    protected override void OnEnable() {
        base.OnEnable();

        shootBehavior.OnShoot_Direction += OnShoot;
    }
    protected override void OnDisable() {
        base.OnDisable();

        shootBehavior.OnShoot_Direction -= OnShoot;
    }

    protected override void OnPlayerEnteredRange(Collider2D playerCol) {
        base.OnPlayerEnteredRange(playerCol);

        wanderMovement.enabled = true;
        wanderMovement.FleePlayer = true;
    }
    protected override void OnPlayerExitedRange(Collider2D playerCol) {
        base.OnPlayerExitedRange(playerCol);

        wanderMovement.enabled = false;
    }

    private void OnShoot(Vector2 direction) {

        //... add recoil force to the opposite direction shot
        rb.AddForce(recoilForce * -direction, ForceMode2D.Impulse);

        recoil.RecoilWeapon();
    }
}
