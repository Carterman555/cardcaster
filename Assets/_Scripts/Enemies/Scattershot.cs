using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scattershot : Enemy {

    [SerializeField] private Transform shootPoint;

    [Header("Movement")]
    private FleePlayerBehavior fleePlayerBehavior;

    [Header("Shoot Projectile")]
    [SerializeField] private RandomInt projectileCount;
    [SerializeField] private StraightMovement projectilePrefab;
    private ShootStraightSpreadBehavior shootBehavior;

    [Header("Recoil")]
    [SerializeField] private float recoilForce;
    private Rigidbody2D rb;

    protected override void Awake() {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();

        InitializeBehaviors();
    }

    protected override void OnEnable() {
        base.OnEnable();

        shootBehavior.OnShoot_Direction += OnShoot;
    }
    protected override void OnDisable() {
        base.OnDisable();

        shootBehavior.OnShoot_Direction -= OnShoot;
    }

    private void InitializeBehaviors() {
        fleePlayerBehavior = new(this);
        enemyBehaviors.Add(fleePlayerBehavior);

        shootBehavior = new(this, projectilePrefab, shootPoint, projectileCount.Randomize());
        enemyBehaviors.Add(shootBehavior);
    }

    protected override void OnPlayerEnteredRange(GameObject player) {
        base.OnPlayerEnteredRange(player);

        if (!MovementStopped) {
            fleePlayerBehavior.Start();
        }

        shootBehavior.Stop();
    }
    protected override void OnPlayerExitedRange(GameObject player) {
        base.OnPlayerExitedRange(player);

        fleePlayerBehavior.Stop();
        shootBehavior.StartShooting(player.transform);
    }

    private void OnShoot(Vector2 direction) {

        // add recoil force to the opposite direction shot
        rb.AddForce(recoilForce * -direction, ForceMode2D.Impulse);
    }

    protected override void Update() {
        base.Update();
    }

    public override void OnRemoveStopMovementEffect() {
        if (playerWithinRange) {
            fleePlayerBehavior.Start();
        }
    }
}
