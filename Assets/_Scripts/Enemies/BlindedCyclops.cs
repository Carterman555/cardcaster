using UnityEngine;

public class BlindedCyclops : Enemy {

    [Header("Shockwave")]
    private ShockwaveBehavior shockwaveBehavior;
    [SerializeField] private BasicProjectile shockwavePrefab;
    [SerializeField] private int shockwaveCount;

    [Header("Slash")]
    private CircleSlashBehavior circleSlashBehavior;
    [SerializeField] private LayerMask playerLayer;

    protected override void OnEnable() {
        base.OnEnable();
        InitializeBehaviors();
    }

    private void InitializeBehaviors() {
        shockwaveBehavior = new();
        circleSlashBehavior = new();

        enemyBehaviors.Add(shockwaveBehavior);
        enemyBehaviors.Add(circleSlashBehavior);

        foreach (var enemyBehavior in enemyBehaviors) {
            enemyBehavior.Initialize(this);
        }

        shockwaveBehavior.Setup(shockwavePrefab, shockwaveCount);
        shockwaveBehavior.Start();

        circleSlashBehavior.Setup(playerLayer);
    }

    protected override void OnPlayerEnteredRange(GameObject player) {
        base.OnPlayerEnteredRange(player);

        shockwaveBehavior.Stop();
        circleSlashBehavior.Start();
    }

    protected override void OnPlayerExitedRange(GameObject player) {
        base.OnPlayerExitedRange(player);

        shockwaveBehavior.Start();
        circleSlashBehavior.Stop();
    }
}
