using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minionc : Enemy {

    private PlayerBasedMoveBehavior moveBehavior;

    private MergeBehavior mergeBehavior;
    [SerializeField] private TriggerContactTracker mergeTracker;

    protected override void OnEnable() {
        base.OnEnable();

        InitializeBehaviors();
    }

    private void InitializeBehaviors() {
        moveBehavior = new();
        enemyBehaviors.Add(moveBehavior);

        mergeBehavior = new();

        enemyBehaviors.Add(mergeBehavior);

        foreach (var enemyBehavior in enemyBehaviors) {
            enemyBehavior.Initialize(this);
        }
    }
}
