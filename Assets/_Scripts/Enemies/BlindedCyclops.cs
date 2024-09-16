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
        shootBehavior = new();
        circleSlashBehavior = new();

        enemyBehaviors.Add(shootBehavior);
        enemyBehaviors.Add(circleSlashBehavior);

        foreach (var enemyBehavior in enemyBehaviors) {
            enemyBehavior.Initialize(this);
        }

        shootBehavior.Setup(shockwavePrefab, shockwaveCount, true);
        shootBehavior.Start();

        circleSlashBehavior.Setup(centerPoint);
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
