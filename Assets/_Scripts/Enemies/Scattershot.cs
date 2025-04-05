using UnityEngine;

public class Scattershot : Enemy {

    private FleePlayerBehavior fleePlayerBehavior;
    private ShootStraightSpreadBehavior shootBehavior;

    [Header("Recoil")]
    [SerializeField] private Recoil recoil;
    [SerializeField] private float recoilForce;
    private Rigidbody2D rb;

    protected override void Awake() {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();

        fleePlayerBehavior = GetComponent<FleePlayerBehavior>();
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

    protected override void OnPlayerEnteredRange(GameObject player) {
        base.OnPlayerEnteredRange(player);

        fleePlayerBehavior.enabled = true;
        shootBehavior.enabled = false;
    }
    protected override void OnPlayerExitedRange(GameObject player) {
        base.OnPlayerExitedRange(player);

        fleePlayerBehavior.enabled = false;
        shootBehavior.enabled = true;
    }

    private void OnShoot(Vector2 direction) {

        //... add recoil force to the opposite direction shot
        rb.AddForce(recoilForce * -direction, ForceMode2D.Impulse);

        recoil.RecoilWeapon();
    }
}
