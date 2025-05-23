using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ElectricMinion : Enemy {

    private NavMeshAgent agent;

    private LineSight lineSight;
    private WanderMovementBehavior wanderMovement;
    private FaceMoveDirectionBehavior faceMoveDirectionBehavior;
    private ChargeBehavior chargeBehavior;
    private FacePlayerBehavior facePlayerBehavior;

    private Rigidbody2D rb;

    protected override void Awake() {
        base.Awake();

        agent = GetComponent<NavMeshAgent>();

        lineSight = GetComponent<LineSight>();
        lineSight.SetTarget(PlayerMovement.Instance.transform);

        wanderMovement = GetComponent<WanderMovementBehavior>();
        wanderMovement.enabled = false;

        faceMoveDirectionBehavior = GetComponent<FaceMoveDirectionBehavior>();
        faceMoveDirectionBehavior.enabled = false;

        chargeBehavior = GetComponent<ChargeBehavior>();
        chargeBehavior.enabled = false;

        facePlayerBehavior = GetComponent<FacePlayerBehavior>();
        facePlayerBehavior.enabled = false;

        rb = GetComponent<Rigidbody2D>();
    }

    protected override void OnEnable() {
        base.OnEnable();

        health.OnDamaged += OnDamaged;
    }

    protected override void OnDisable() {
        base.OnDisable();

        health.OnDamaged -= OnDamaged;
    }

    private void OnDamaged() {
        if (chargeBehavior.enabled) {
            rb.velocity = Vector2.zero;
        }
    }

    protected override void Update() {
        base.Update();

        if (lineSight.TargetInSight && !chargeBehavior.enabled) {
            agent.enabled = false;

            wanderMovement.enabled = false;
            faceMoveDirectionBehavior.enabled = false;

            chargeBehavior.enabled = true;
            facePlayerBehavior.enabled = true;
        }
        else if (!lineSight.TargetInSight && !wanderMovement.enabled && chargeBehavior.CurrentState == ChargeBehavior.ChargeState.OnCooldown) {
            agent.enabled = true;

            wanderMovement.enabled = true;
            faceMoveDirectionBehavior.enabled = true;

            chargeBehavior.enabled = false;
            facePlayerBehavior.enabled = false;
        }
    }
}
