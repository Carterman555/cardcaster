using UnityEngine;

public class BlindedCyclops : Enemy {

    [Header("Shockwave")]
    private CircleStraightShootBehavior shootBehavior;
    [SerializeField] private StraightMovement shockwavePrefab;
    [SerializeField] private int shockwaveCount;

    [Header("Slash")]
    private CircleSlashBehavior circleSlashBehavior;
    [SerializeField] private Transform centerPoint;

    protected override void OnEnable() {
        base.OnEnable();
        InitializeBehaviors();
    }

    private void InitializeBehaviors() {
        float attackCooldownMult = 2f;
        shootBehavior = new(this, shockwavePrefab, shockwaveCount, true, attackCooldownMult);
        enemyBehaviors.Add(shootBehavior);
        shootBehavior.Start();

        circleSlashBehavior = new(this, centerPoint);
        enemyBehaviors.Add(circleSlashBehavior);
    }

    protected override void OnPlayerEnteredRange(GameObject player) {
        base.OnPlayerEnteredRange(player);

        shootBehavior.Stop();
        circleSlashBehavior.Start();
    }

    protected override void OnPlayerExitedRange(GameObject player) {
        base.OnPlayerExitedRange(player);

        shootBehavior.Start();
        circleSlashBehavior.Stop();
    }
}
