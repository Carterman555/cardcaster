using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoomCharger : Enemy {

    private ChasePlayerBehavior moveBehavior;
    private ExplodeBehavior explodeBehavior;
    [SerializeField] private float explosionRadius;
    [SerializeField] private float explosionDelay;

    private bool exploding;

    [Header("Visual")]
    [SerializeField] private ParticleSystem explosionParticlesPrefab;

    protected override void Awake() {
        base.Awake();
        InitializeBehaviors();
    }

    protected override void OnEnable() {
        base.OnEnable();
        moveBehavior.Start();
        exploding = false;
    }

    private void InitializeBehaviors() {
        moveBehavior = new(this);
        enemyBehaviors.Add(moveBehavior);

        explodeBehavior = new(this);
        enemyBehaviors.Add(explodeBehavior);
    }

    protected override void Update() {
        base.Update();
        if (playerWithinRange && !health.IsDead() && !exploding) {
            moveBehavior.Stop();
            StartCoroutine(DelayedExplode());
        }
    }

    private IEnumerator DelayedExplode() {

        exploding = true;

        yield return new WaitForSeconds(explosionDelay);

        explodeBehavior.Explode(GameLayers.PlayerLayerMask, explosionRadius);
        explosionParticlesPrefab.Spawn(transform.position, Containers.Instance.Effects);
        gameObject.ReturnToPool();

        exploding = false;
    }

    protected override void OnDisable() {
        base.OnDisable();
        StopAllCoroutines();
    }
}
