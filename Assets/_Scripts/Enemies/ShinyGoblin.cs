using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShinyGoblin : Enemy {

    [SerializeField] private RandomFloat fleeDelay;
    private float fleeTimer;

    private WanderMovementBehavior wanderMovement;

    [SerializeField] private StatsBall statsBallPrefab;

    protected override void Awake() {
        base.Awake();

        wanderMovement = GetComponent<WanderMovementBehavior>();
    }

    protected override void OnEnable() {
        base.OnEnable();

        health.DeathEventTrigger.AddListener(SpawnStatBoosts);

        fleeTimer = 0;
        fleeDelay.Randomize();
    }

    protected override void OnDisable() {
        base.OnDisable();
        health.DeathEventTrigger.RemoveListener(SpawnStatBoosts);
    }

    protected override void Update() {
        base.Update();

        fleeTimer += Time.deltaTime;
        if (fleeTimer > fleeDelay.Value) {
            fleeTimer = 0;

            wanderMovement.enabled = false;

            anim.SetTrigger("flee");
        }
    }

    private void SpawnStatBoosts() {
        statsBallPrefab.Spawn(transform.position, Containers.Instance.Drops);
    }

    // played by flee anim
    public void ReturnToPool() {
        gameObject.ReturnToPool();
    }
}
