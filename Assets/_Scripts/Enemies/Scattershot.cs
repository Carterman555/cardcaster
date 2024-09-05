using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scattershot : Enemy {


    [Header("Weapon")]
    [SerializeField] private Transform shootPoint;
    [SerializeField] private Animator weaponAnim;
    [SerializeField] private PointTowardsPlayer weaponPoint;
    [SerializeField] private Recoil weaponRecoil;

    [Header("Movement")]
    private FleePlayerBehavior fleePlayerBehavior;

    //private ChangeFacingBehavior changeFacingBehavior;

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
    }

    protected override void OnEnable() {
        base.OnEnable();
        InitializeBehaviors();

        shootBehavior.OnShoot_Direction += OnShoot;
    }
    protected override void OnDisable() {
        base.OnDisable();

        shootBehavior.OnShoot_Direction -= OnShoot;
    }

    private void InitializeBehaviors() {

        fleePlayerBehavior = new();
        fleePlayerBehavior.Initialize(this);
        fleePlayerBehavior.Stop();
        enemyBehaviors.Add(fleePlayerBehavior);

        shootBehavior = new();
        shootBehavior.Setup(projectilePrefab, shootPoint.localPosition, projectileCount.Randomize());
        enemyBehaviors.Add(shootBehavior);

        foreach (var enemyBehavior in enemyBehaviors) {
            enemyBehavior.Initialize(this);
        }
    }

    protected override void OnPlayerEnteredRange(GameObject player) {
        base.OnPlayerEnteredRange(player);

        fleePlayerBehavior.Start();
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

        weaponRecoil.RecoilWeapon();
    }
}
