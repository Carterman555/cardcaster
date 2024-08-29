using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwistedGoblin : Enemy {

    private ChasePlayerBehavior moveBehavior;

    [Header("Attack")]
    [SerializeField] private SlashingWeapon weapon;
    [SerializeField] private LayerMask targetLayerMask;
    [SerializeField] private float slashSize;
    private SlashAttackBehavior slashBehavior;

    protected override void OnEnable() {
        base.OnEnable();
        InitializeBehaviors();

        moveBehavior.Start();
    }

    private void InitializeBehaviors() {
        moveBehavior = new();
        enemyBehaviors.Add(moveBehavior);

        slashBehavior = new();
        slashBehavior.Setup(weapon, targetLayerMask, slashSize);
        enemyBehaviors.Add(slashBehavior);

        foreach (var enemyBehavior in enemyBehaviors) {
            enemyBehavior.Initialize(this);
        }
    }

    protected override void OnPlayerEnteredRange(GameObject player) {
        base.OnPlayerEnteredRange(player);

        moveBehavior.Stop();
        slashBehavior.StartAttacking();
    }

    protected override void OnPlayerExitedRange(GameObject player) {
        base.OnPlayerExitedRange(player);

        moveBehavior.Start();
        slashBehavior.StopAttacking();
    }
}
