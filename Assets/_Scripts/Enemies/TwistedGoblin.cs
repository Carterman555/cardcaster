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

        InitializeBehaviors();
    }

    protected override void OnEnable() {
        base.OnEnable();

        moveBehavior.Start();
    }

    private void InitializeBehaviors() {
        moveBehavior = new(this);
        enemyBehaviors.Add(moveBehavior);

        slashBehavior = new(this, centerPoint);
        enemyBehaviors.Add(slashBehavior);
    }

    protected override void OnPlayerEnteredRange(GameObject player) {
        base.OnPlayerEnteredRange(player);

        moveBehavior.Stop();
        slashBehavior.Start();
    }

    protected override void OnPlayerExitedRange(GameObject player) {
        base.OnPlayerExitedRange(player);

        if (!TryGetComponent(out StopMovement stopMovement)) {
            moveBehavior.Start();
        }

        slashBehavior.Stop();
    }

    public override void OnAddEffect(UnitEffect unitEffect) {
        base.OnAddEffect(unitEffect);

        if (unitEffect is StopMovement) {
            if (!playerWithinRange) {
                moveBehavior.Start();
            }
        }
    }
}
