using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwistedGoblin : Enemy {

    private ChasePlayerBehavior moveBehavior;

    [Header("Attack")]
    [SerializeField] private Transform centerPoint;
    private CircleSlashBehavior slashBehavior;

    protected override void Awake() {
        base.Awake();

        moveBehavior = GetComponent<ChasePlayerBehavior>();

        InitializeBehaviors();
    }

    protected override void OnEnable() {
        base.OnEnable();

        moveBehavior.enabled = true;
    }

    private void InitializeBehaviors() {
        slashBehavior = new(this, centerPoint);
        enemyBehaviors.Add(slashBehavior);
    }

    protected override void OnPlayerEnteredRange(GameObject player) {
        base.OnPlayerEnteredRange(player);

        moveBehavior.enabled = false;
        slashBehavior.Start();
    }

    protected override void OnPlayerExitedRange(GameObject player) {
        base.OnPlayerExitedRange(player);

        if (!TryGetComponent(out StopMovement stopMovement)) {
            moveBehavior.enabled = true;
        }

        slashBehavior.Stop();
    }

    public override void OnAddEffect(UnitEffect unitEffect) {
        base.OnAddEffect(unitEffect);

        if (unitEffect is StopMovement) {
            if (!playerWithinRange) {
                moveBehavior.enabled = true;
            }
        }
    }
}
