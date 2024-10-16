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

    protected override void Awake() {
        base.Awake();

        lineSight = new(PlayerMovement.Instance.transform, sightLayerMask, castRadius);

        chasePlayerBehavior = GetComponent<ChasePlayerBehavior>();
        shootBehavior = GetComponent<StraightShootBehavior>();

        EnterChaseState();
    }

    protected override void Update() {
        base.Update();

        if (lineSight.InSight(transform.position) && !playerInSight) {
            EnterShootState();
        }
        else if (!lineSight.InSight(transform.position) && playerInSight) {
            EnterChaseState();
        }
    }

    public override void OnAddEffect(UnitEffect unitEffect) {
        base.OnAddEffect(unitEffect);

        if (unitEffect is StopMovement) {
            if (!lineSight.InSight(transform.position)) {
                EnterChaseState();
            }
        }
    }

    private void EnterChaseState() {
        chasePlayerBehavior.enabled = true;
        shootBehavior.enabled = false;

        playerInSight = false;
    }

    private void EnterShootState() {
        chasePlayerBehavior.enabled = false;
        shootBehavior.enabled = true;

        playerInSight = true;
    }
}
