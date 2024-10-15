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

        moveBehavior = GetComponent<ChasePlayerBehavior>();

        InitializeBehaviors();
    }

    protected override void OnEnable() {
        base.OnEnable();

        moveBehavior.enabled = true;

        shootBehavior.OnShootAnim += BowShootAnim;
    }

    protected override void OnDisable() {
        base.OnDisable();

        shootBehavior.OnShootAnim += BowShootAnim;
    }

    private void InitializeBehaviors() {
        shootBehavior = new(this, projectilePrefab, shootPoint);
        enemyBehaviors.Add(shootBehavior);
    }

    protected override void OnPlayerEnteredRange(GameObject player) {
        base.OnPlayerEnteredRange(player);

        moveBehavior.enabled = false;
        shootBehavior.StartShooting(player.transform);
    }

    protected override void OnPlayerExitedRange(GameObject player) {
        base.OnPlayerExitedRange(player);

        moveBehavior.enabled = true;
        shootBehavior.Stop();
    }

    public override void OnAddEffect(UnitEffect unitEffect) {
        base.OnAddEffect(unitEffect);

        if (unitEffect is StopMovement) {
            if (!playerWithinRange) {
                moveBehavior.enabled = true;
            }
        }
    }

    private void BowShootAnim() {
        anim.SetTrigger("shoot");
    }
}
