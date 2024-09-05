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
        moveBehavior = new();
        shootBehavior = new();

        enemyBehaviors.Add(moveBehavior);
        enemyBehaviors.Add(shootBehavior);

        foreach (var enemyBehavior in enemyBehaviors) {
            enemyBehavior.Initialize(this);
        }

        moveBehavior.Setup(moveRadius);
        moveBehavior.Start();

        shootBehavior.Setup(projectilePrefab, projectileCount);
        shootBehavior.Start();
    }
}
