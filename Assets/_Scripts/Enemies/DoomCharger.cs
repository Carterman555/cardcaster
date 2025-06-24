using System.Collections;
using UnityEngine;

public class DoomCharger : Enemy {

    private ChasePlayerBehavior moveBehavior;
    private ExplodeBehavior explodeBehavior;

    [SerializeField] private float explosionDelay;

    private bool exploding;

    protected override void Awake() {
        base.Awake();

        moveBehavior = GetComponent<ChasePlayerBehavior>();
        explodeBehavior = GetComponent<ExplodeBehavior>();

        ExplosionTarget playerExplosionTarget = new() {
            LayerMask = GameLayers.PlayerLayerMask,
            ExplosionRadius = 3f,
            Damage = GetEnemyStats().Damage,
            KnockbackStrength = GetEnemyStats().KnockbackStrength
        };

        explodeBehavior.AddedExplosionTargets.Add(playerExplosionTarget);
    }

    protected override void OnEnable() {
        base.OnEnable();
        moveBehavior.enabled = true;
        exploding = false;
    }

    protected override void OnDisable() {
        base.OnDisable();

        // stop exploded if killed
        StopAllCoroutines();
    }

    protected override void Update() {
        base.Update();
        if (playerWithinRange && !health.Dead && !exploding) {
            moveBehavior.enabled = false;
            StartCoroutine(DelayedExplode());
        }
    }

    private IEnumerator DelayedExplode() {

        exploding = true;

        yield return new WaitForSeconds(explosionDelay);

        explodeBehavior.Explode(gameObject);
        gameObject.ReturnToPool();

        exploding = false;
    }
}
