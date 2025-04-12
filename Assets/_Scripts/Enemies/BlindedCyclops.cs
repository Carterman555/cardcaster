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

        //... get the timer value before the behaviour is disabled because it resets the timer
        float timer = shootBehavior.GetActionTimer();

        shootBehavior.enabled = false;
        circleSlashBehavior.enabled = true;

        // keep the same cooldown so player cannot prevent attack by going in and out of cyclops range
        circleSlashBehavior.SetActionTimeRemaining(timer);
    }

    protected override void OnPlayerExitedRange(GameObject player) {
        base.OnPlayerExitedRange(player);

        //... get the timer value before the behaviour is disabled because it resets the timer
        float timer = circleSlashBehavior.GetActionTimer();

        shootBehavior.enabled = true;
        circleSlashBehavior.enabled = false;

        // keep the same cooldown so player cannot prevent attack by going in and out of cyclops range
        shootBehavior.SetActionTimer(timer);
    }
}
