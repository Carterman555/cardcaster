using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoomCharger : Enemy {

    private ChasePlayerBehavior moveBehavior;
    private ExplodeBehavior explodeBehavior;
    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] private float explosionRadius;

    protected override void OnEnable() {
        base.OnEnable();
        InitializeBehaviors();

        moveBehavior.Start();
    }

    private void InitializeBehaviors() {
        moveBehavior = new();
        enemyBehaviors.Add(moveBehavior);

        explodeBehavior = new();
        enemyBehaviors.Add(explodeBehavior);

        foreach (var enemyBehavior in enemyBehaviors) {
            enemyBehavior.Initialize(this);
        }
    }

    protected override void Update() {
        base.Update();
        if (playerWithinRange && !health.IsDead()) {
            explodeBehavior.Explode(playerLayerMask, explosionRadius);
            health.Die();
        }
    }

    protected override void OnDisable() {
        base.OnDisable();
    }
}
