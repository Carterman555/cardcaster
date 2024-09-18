using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaneBlaster : Enemy {

    private ChasePlayerBehavior chasePlayerBehavior;

    [Header("Shoot Projectile")]
    [SerializeField] private Transform shootPoint;
    [SerializeField] private StraightMovement projectilePrefab;
    [SerializeField] private Animator blasterAnim;
    private StraightShootBehavior shootBehavior;

    [Header("Sight")]
    [SerializeField] private LayerMask sightLayerMask;
    [SerializeField] private float castRadius;
    private ObjectInLineSight lineSight;

    private bool playerInSight;

    #region Initialization

    protected override void Awake() {
        base.Awake();
        InitializeBehaviors();
        InitializeSensors();
    }

    protected override void OnEnable() {
        base.OnEnable();
        shootBehavior.OnShootAnim += PlayShootAnimation;
    }

    protected override void OnDisable() {
        base.OnDisable();

        shootBehavior.OnShootAnim -= PlayShootAnimation;
    }

    private void InitializeBehaviors() {

        chasePlayerBehavior = new(this);
        enemyBehaviors.Add(chasePlayerBehavior);

        shootBehavior = new(this, projectilePrefab, shootPoint);
        enemyBehaviors.Add(shootBehavior);
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

    private void PlayShootAnimation() {
        blasterAnim.SetTrigger("shoot");
    }
}
