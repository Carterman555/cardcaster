using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScarredSkeleton : Enemy {

    private ChasePlayerBehavior moveBehavior;

    [Header("Attack")]
    private StraightShootBehavior shootBehavior;
    [SerializeField] private BasicProjectile projectile;
    [SerializeField] private Transform shootPoint;

    protected override void OnEnable() {
        base.OnEnable();
        InitializeBehaviors();

        moveBehavior.Start();
    }

    private void InitializeBehaviors() {
        moveBehavior = new();
        enemyBehaviors.Add(moveBehavior);

        shootBehavior = new();
        shootBehavior.Setup(projectile, shootPoint.localPosition);
        enemyBehaviors.Add(shootBehavior);

        foreach (var enemyBehavior in enemyBehaviors) {
            enemyBehavior.Initialize(this);
        }
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
}
