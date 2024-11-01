using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScarredSkeleton : Enemy {

    private ChasePlayerBehavior moveBehavior;
    private StraightShootBehavior shootBehavior;

    protected override void Awake() {
        base.Awake();

        moveBehavior = GetComponent<ChasePlayerBehavior>();
        shootBehavior = GetComponent<StraightShootBehavior>();
    }

    protected override void OnEnable() {
        base.OnEnable();

        moveBehavior.enabled = true;
        shootBehavior.enabled = false;
    }

    protected override void OnPlayerEnteredRange(GameObject player) {
        base.OnPlayerEnteredRange(player);

        moveBehavior.enabled = false;
        shootBehavior.enabled = true;
    }

    protected override void OnPlayerExitedRange(GameObject player) {
        base.OnPlayerExitedRange(player);

        moveBehavior.enabled = true;
        shootBehavior.enabled = false;
    }
}
