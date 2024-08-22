using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingRusher : Enemy {

    private ChasePlayerBehavior chasePlayerBehavior;
    private ExplodeBehavior explodeBehavior;
    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] private float explosionRadius;

    private Health health;

    private void Awake() {
        health = GetComponent<Health>();
    }

    private void OnEnable() {
        InitializeBehaviors();

        chasePlayerBehavior.Start();
    }

    private void InitializeBehaviors() {
        chasePlayerBehavior = new();
        chasePlayerBehavior.SetSpeed(stats.MoveSpeed);
        enemyBehaviors.Add(chasePlayerBehavior);

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
        chasePlayerBehavior.Stop();
    }
}
