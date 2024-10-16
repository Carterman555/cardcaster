using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwistedGoblin : Enemy {

    private ChasePlayerBehavior moveBehavior;
    private CircleSlashBehavior slashBehavior;

    protected override void Awake() {
        base.Awake();

        moveBehavior = GetComponent<ChasePlayerBehavior>();
        slashBehavior = GetComponent<CircleSlashBehavior>();

        moveBehavior.enabled = true;
        slashBehavior.enabled = false;
    }

    protected override void OnPlayerEnteredRange(GameObject player) {
        base.OnPlayerEnteredRange(player);

        moveBehavior.enabled = false;
        slashBehavior.enabled = true;
    }

    protected override void OnPlayerExitedRange(GameObject player) {
        base.OnPlayerExitedRange(player);

        moveBehavior.enabled = true;
        slashBehavior.enabled = false;
    }
}
