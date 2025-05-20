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

    protected override void OnPlayerEnteredRange(Collider2D playerCol) {
        base.OnPlayerEnteredRange(playerCol);

        moveBehavior.enabled = false;
        slashBehavior.enabled = true;
    }

    protected override void OnPlayerExitedRange(Collider2D playerCol) {
        base.OnPlayerExitedRange(playerCol);

        moveBehavior.enabled = true;
        slashBehavior.enabled = false;
    }
}
