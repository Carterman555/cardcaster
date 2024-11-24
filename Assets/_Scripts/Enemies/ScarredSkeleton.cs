using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScarredSkeleton : Enemy {

    private ChasePlayerBehavior chasePlayerBehavior;
    private StraightShootBehavior shootBehavior;
    private LineSight lineSight;

    protected override void Awake() {
        base.Awake();

        chasePlayerBehavior = GetComponent<ChasePlayerBehavior>();
        shootBehavior = GetComponent<StraightShootBehavior>();
        lineSight = GetComponent<LineSight>();

        lineSight.SetTarget(PlayerMeleeAttack.Instance.transform);

        EnterChaseState();
    }

    protected override void OnEnable() {
        base.OnEnable();

        lineSight.OnEnterSight += EnterShootState;
        lineSight.OnExitSight += EnterChaseState;
    }

    protected override void OnDisable() {
        base.OnDisable();

        lineSight.OnEnterSight -= EnterShootState;
        lineSight.OnExitSight -= EnterChaseState;
    }

    private void EnterChaseState() {
        chasePlayerBehavior.enabled = true;
        shootBehavior.enabled = false;
    }

    private void EnterShootState() {
        chasePlayerBehavior.enabled = false;
        shootBehavior.enabled = true;
    }

    public override void OnAddEffect(UnitEffect unitEffect) {
        base.OnAddEffect(unitEffect);

        if (unitEffect is StopMovement) {
            if (!lineSight.InSight()) {
                EnterChaseState();
            }
        }
    }
}
