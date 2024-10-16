using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaneBlaster : Enemy {

    private ChasePlayerBehavior chasePlayerBehavior;
    private StraightShootBehavior shootBehavior;

    [Header("Sight")]
    [SerializeField] private LayerMask sightLayerMask;
    [SerializeField] private float castRadius;
    private ObjectInLineSight lineSight;

    private bool playerInSight;

    #region Initialization

    protected override void Awake() {
        base.Awake();
        InitializeSensors();

        chasePlayerBehavior = GetComponent<ChasePlayerBehavior>();
        shootBehavior = GetComponent<StraightShootBehavior>();
    }

    private void InitializeSensors() {
        lineSight = new(PlayerMovement.Instance.transform, sightLayerMask, castRadius);
    }
    #endregion

    protected override void Update() {
        base.Update();

        if (lineSight.InSight(transform.position) && !playerInSight) {
            chasePlayerBehavior.enabled = false;
            shootBehavior.enabled = true;
            shootBehavior.StartShooting(PlayerMovement.Instance.transform);

            playerInSight = true;
        }
        else if (!lineSight.InSight(transform.position) && playerInSight) {
            chasePlayerBehavior.enabled = true;
            shootBehavior.enabled = false;

            playerInSight = false;
        }
    }

    public override void OnAddEffect(UnitEffect unitEffect) {
        base.OnAddEffect(unitEffect);

        if (unitEffect is StopMovement) {
            if (!lineSight.InSight(transform.position)) {
                chasePlayerBehavior.enabled = true;
                shootBehavior.enabled = false;

                playerInSight = false;
            }
        }
    }
}
