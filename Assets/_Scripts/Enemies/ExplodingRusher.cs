using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingRusher : Enemy {

    private PlayerBasedMoveBehavior moveBehavior;
    private ExplodeBehavior explodeBehavior;
    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] private float explosionRadius;

    private Health health;

    private void Awake() {
        health = GetComponent<Health>();
    }

    protected override void OnEnable() {
        base.OnEnable();
        InitializeBehaviors();

        moveBehavior.Start();
    }

    private void InitializeBehaviors() {
        moveBehavior = new();
        moveBehavior.SetSpeed(stats.MoveSpeed);
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
            explodeBehavior.Explode(playerLayerMask, explosionRadius, stats.Damage);
            health.Die();
        }
    }

    private void OnDisable() {
        moveBehavior.Stop();
    }
}
