using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BabyBeholder : Enemy {

    private ChasePlayerBehavior chasePlayerBehavior;

    [Header("Shoot Projectile")]
    [SerializeField] private Transform shootPoint;
    [SerializeField] private StraightMovement projectilePrefab;
    private StraightShootBehavior shootBehavior;

    [Header("Sight")]
    [SerializeField] private LayerMask sightLayerMask;
    [SerializeField] private float castRadius;
    private ObjectInLineSight lineSight;

    private bool playerInSight;

    #region Initialization
    protected override void OnEnable() {
        base.OnEnable();
        InitializeBehaviors();
        InitializeSensors();
    }

    private void InitializeBehaviors() {

        chasePlayerBehavior = new();
        chasePlayerBehavior.Initialize(this);
        enemyBehaviors.Add(chasePlayerBehavior);

        shootBehavior = new();
        shootBehavior.Setup(projectilePrefab, shootPoint.localPosition);
        enemyBehaviors.Add(shootBehavior);

        foreach (var enemyBehavior in enemyBehaviors) {
            enemyBehavior.Initialize(this);
        }
    }

    private void InitializeSensors() {
        lineSight = new(PlayerMovement.Instance.transform, sightLayerMask, castRadius);
    }
    #endregion

    protected override void Update() {
        base.Update();

        if (lineSight.InSight(transform.position) && !playerInSight) {
            chasePlayerBehavior.Stop();
            shootBehavior.StartShooting(PlayerMovement.Instance.transform);

            playerInSight = true;
        }
        else if (!lineSight.InSight(transform.position) && playerInSight) {
            chasePlayerBehavior.Start();
            shootBehavior.Stop();

            playerInSight = false;
        }
    }

    public override void OnRemoveStopMovementEffect() {
        base.OnRemoveStopMovementEffect();

        if (!lineSight.InSight(transform.position)) {
            chasePlayerBehavior.Start();
            shootBehavior.Stop();

            playerInSight = false;
        }
    }
}
