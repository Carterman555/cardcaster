using UnityEngine;

public class BlindedCyclops : Enemy {

    private CircleStraightShootBehavior shootBehavior;
    private CircleSlashBehavior circleSlashBehavior;

    protected override void Awake() {
        base.Awake();

        shootBehavior = GetComponent<CircleStraightShootBehavior>();
        circleSlashBehavior = GetComponent<CircleSlashBehavior>();
    }

    protected override void OnEnable() {
        base.OnEnable();

        shootBehavior.enabled = true;
        circleSlashBehavior.enabled = false;
    }

    protected override void OnPlayerEnteredRange(GameObject player) {
        base.OnPlayerEnteredRange(player);

        shootBehavior.enabled = false;
        circleSlashBehavior.enabled = true;
    }

    protected override void OnPlayerExitedRange(GameObject player) {
        base.OnPlayerExitedRange(player);

        shootBehavior.enabled = true;
        circleSlashBehavior.enabled = false;
    }
}
