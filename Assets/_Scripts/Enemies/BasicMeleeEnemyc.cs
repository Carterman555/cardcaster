using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMeleeEnemyc : Enemy {

    private PlayerBasedMoveBehavior moveBehavior;

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
        moveBehavior.SetSpeed(stats.MoveSpeed);
        enemyBehaviors.Add(moveBehavior);

        slashBehavior = new();
        slashBehavior.Setup(weapon, targetLayerMask, slashSize, stats.AttackCooldown, stats.Damage);
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
