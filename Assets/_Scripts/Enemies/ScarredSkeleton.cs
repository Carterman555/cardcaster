using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScarredSkeleton : Enemy {

    private ChasePlayerBehavior moveBehavior;

    [Header("Attack")]
    private StraightShootBehavior shootBehavior;
    [SerializeField] private StraightMovement projectilePrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private Animator anim;

    protected override void Awake() {
        base.Awake();

        InitializeBehaviors();
    }

    protected override void OnEnable() {
        base.OnEnable();

        moveBehavior.Start();

        shootBehavior.OnShootAnim += BowShootAnim;
    }

    protected override void OnDisable() {
        base.OnDisable();

        shootBehavior.OnShootAnim += BowShootAnim;
    }

    private void InitializeBehaviors() {
        moveBehavior = new(this);
        enemyBehaviors.Add(moveBehavior);

        shootBehavior = new(this, projectilePrefab, shootPoint);
        enemyBehaviors.Add(shootBehavior);
    }

    protected override void OnPlayerEnteredRange(GameObject player) {
        base.OnPlayerEnteredRange(player);

        moveBehavior.Stop();
        shootBehavior.StartShooting(player.transform);
    }

    protected override void OnPlayerExitedRange(GameObject player) {
        base.OnPlayerExitedRange(player);

        moveBehavior.Start();
        shootBehavior.Stop();
    }

    public override void OnAddEffect(UnitEffect unitEffect) {
        base.OnAddEffect(unitEffect);

        if (unitEffect is StopMovement) {
            if (!playerWithinRange) {
                moveBehavior.Start();
            }
        }
    }

    private void BowShootAnim() {
        anim.SetTrigger("shoot");
    }
}
