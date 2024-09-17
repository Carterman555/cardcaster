using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ace : Enemy {

    [Header("Movement")]
    private CircleMoveBehavior moveBehavior;
    [SerializeField] private float moveRadius;

    [Header("Attack")]
    private CircleStraightShootBehavior shootBehavior;
    [SerializeField] private StraightMovement projectilePrefab;
    [SerializeField] private int projectileCount;

    protected override void OnEnable() {
        base.OnEnable();
        InitializeBehaviors();
    }

    private void InitializeBehaviors() {
        moveBehavior = new(this, moveRadius);
        enemyBehaviors.Add(moveBehavior);
        moveBehavior.Start();

        shootBehavior = new(this, projectilePrefab, projectileCount, false);
        enemyBehaviors.Add(shootBehavior);
        shootBehavior.Start();
    }
}
