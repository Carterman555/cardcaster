using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScarredSkeleton : Enemy {

    private ChasePlayerBehavior moveBehavior;

    [Header("Attack")]
    private StraightShootBehavior shootBehavior;
    [SerializeField] private StraightMovement projectilePrefab;
    [SerializeField] private Transform shootPoint;

    protected override void OnEnable() {
        base.OnEnable();
        InitializeBehaviors();

        moveBehavior.Start();
    }

    private void InitializeBehaviors() {
        moveBehavior = new(this);
        enemyBehaviors.Add(moveBehavior);

        shootBehavior = new(this, projectilePrefab, shootPoint.localPosition);
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

    public override void OnRemoveStopMovementEffect() {
        if (!playerWithinRange) {
            moveBehavior.Start();
        }
    }
}
