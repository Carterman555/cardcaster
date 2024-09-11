using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwistedGoblin : Enemy {

    private ChasePlayerBehavior moveBehavior;

    [Header("Attack")]
    [SerializeField] private SlashingWeapon weapon;
    [SerializeField] private LayerMask targetLayerMask;
    [SerializeField] private float slashSize;
    private SwordSlashBehavior slashBehavior;

    protected override void OnEnable() {
        base.OnEnable();
        InitializeBehaviors();

        moveBehavior.Start();
    }

    private void InitializeBehaviors() {
        moveBehavior = new();
        slashBehavior = new();

        enemyBehaviors.Add(moveBehavior);
        enemyBehaviors.Add(slashBehavior);

        foreach (var enemyBehavior in enemyBehaviors) {
            enemyBehavior.Initialize(this);
        }

        slashBehavior.Setup(weapon, targetLayerMask, slashSize);
    }

    protected override void OnPlayerEnteredRange(GameObject player) {
        base.OnPlayerEnteredRange(player);

        moveBehavior.Stop();
        slashBehavior.Start();
    }

    protected override void OnPlayerExitedRange(GameObject player) {
        base.OnPlayerExitedRange(player);

        if (!MovementStopped) {
            moveBehavior.Start();
        }

        slashBehavior.Stop();
    }

    public override void OnRemoveStopMovementEffect() {
        if (!playerWithinRange) {
            moveBehavior.Start();
        }
    }
}
