using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ElectricMinion : Enemy {

    private NavMeshAgent agent;

    private LineSight lineSight;
    private WanderMovementBehavior wanderMovement;
    private ChargeBehavior chargeBehavior;

    protected override void Awake() {
        base.Awake();

        agent = GetComponent<NavMeshAgent>();

        lineSight = GetComponent<LineSight>();
        lineSight.SetTarget(PlayerMovement.Instance.transform);

        wanderMovement = GetComponent<WanderMovementBehavior>();
        wanderMovement.enabled = false;

        chargeBehavior = GetComponent<ChargeBehavior>();
        chargeBehavior.enabled = false;
    }

    protected override void Update() {
        base.Update();

        if (lineSight.TargetInSight && !chargeBehavior.enabled) {
            agent.enabled = false;

            wanderMovement.enabled = false;
            chargeBehavior.enabled = true;
        }
        else if (!lineSight.TargetInSight && !wanderMovement.enabled && chargeBehavior.CurrentState == ChargeBehavior.ChargeState.OnCooldown) {
            agent.enabled = true;

            wanderMovement.enabled = true;
            chargeBehavior.enabled = false;
        }
    }
}
