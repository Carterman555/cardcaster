using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TripletOfDoom : Enemy {

    private WanderMovementBehavior wanderMovement;
    private ChasePlayerBehavior chasePlayerMovement;

    [SerializeField] private float distanceToFlee;

    protected override void Awake() {
        base.Awake();

        wanderMovement = GetComponent<WanderMovementBehavior>();
        chasePlayerMovement = GetComponent<ChasePlayerBehavior>();
    }

    protected override void Update() {
        base.Update();

        HandleMovement();
        HandleAttack();
    }

    #region Movement

    protected override void OnPlayerEnteredRange(Collider2D playerCol) {
        base.OnPlayerEnteredRange(playerCol);

        Wander();
    }

    protected override void OnPlayerExitedRange(Collider2D playerCol) {
        base.OnPlayerExitedRange(playerCol);

        ChasePlayer();
    }


    private void HandleMovement() {
        if (playerWithinRange) {
            float playerDistanceSquared = Vector2.SqrMagnitude(PlayerMovement.Instance.CenterPos - transform.position);
            float distanceToFleeSquared = distanceToFlee * distanceToFlee;

            bool playerWithinFleeDistance = playerDistanceSquared < distanceToFleeSquared;
            wanderMovement.FleePlayer = playerWithinFleeDistance;
        }
    }

    private void Wander() {
        wanderMovement.enabled = true;
        chasePlayerMovement.enabled = false;
    }

    private void ChasePlayer() {
        wanderMovement.enabled = false;
        chasePlayerMovement.enabled = true;
    }

    #endregion

    #region Shoot Exploding Skulls

    [SerializeField] private HeatSeekMovement skullsProjectilePrefab;
    [SerializeField] private Transform shootPoint;

    private float shootTimer;

    private void HandleAttack() {

        shootTimer += Time.deltaTime;
        if (shootTimer > EnemyStats.AttackCooldown) {
            HeatSeekMovement projectile = skullsProjectilePrefab.Spawn(shootPoint.position, Containers.Instance.Projectiles);
            projectile.Setup(PlayerMovement.Instance.CenterTransform);
        }
    }

    #endregion
}
