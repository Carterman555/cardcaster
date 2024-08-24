using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Minion : Enemy, IMergable {

    private PlayerBasedMoveBehavior moveBehavior;

    private MergeBehavior mergeBehavior;
    [SerializeField] private TriggerContactTracker mergeTracker;
    [SerializeField] private Enemy mergedEnemy;
    [SerializeField] private float toMergeDelay;
    [SerializeField] private float mergeTime;

    public GameObject GetObject() {
        return gameObject;
    }

    public MergeBehavior GetMergeBehavior() {
        return mergeBehavior;
    }

    protected override void OnEnable() {
        base.OnEnable();

        InitializeBehaviors();
    }

    private void InitializeBehaviors() {
        moveBehavior = new();
        enemyBehaviors.Add(moveBehavior);

        mergeBehavior = new();
        mergeBehavior.Setup(mergeTracker, mergedEnemy, toMergeDelay, mergeTime);
        mergeBehavior.AllowMerging();
        enemyBehaviors.Add(mergeBehavior);

        foreach (var enemyBehavior in enemyBehaviors) {
            enemyBehavior.Initialize(this);
        }
    }

    protected override void Update() {
        base.Update();

        // stop chasing the player when merging
        if (mergeBehavior.IsMovingToMerge() || mergeBehavior.IsMerging()) {
            if (!moveBehavior.IsStopped()) {
                moveBehavior.Stop();
            }
        }
        else {
            if (moveBehavior.IsStopped()) {
                moveBehavior.Start();
            }
        }

        // if the enemy touches the merging partner, then start merging
        if (mergeBehavior.IsMovingToMerge()) {

            // if this enemy is touching it's merging partner
            if (mergablesTouching.Any(mergable => mergeBehavior.GetMergingPartner().Equals(mergable))) {
                mergeBehavior.StartMerging();
            }
        }
    }

    // only merges when not close to player
    protected override void OnPlayerEnteredRange(GameObject player) {
        base.OnPlayerEnteredRange(player);

        mergeBehavior.DontAllowMerging();
    }

    protected override void OnPlayerExitedRange(GameObject player) {
        base.OnPlayerExitedRange(player);

        mergeBehavior.AllowMerging();
    }

    // if the enemy touches the merging partner, then start merging
    private List<IMergable> mergablesTouching = new();

    private void OnTriggerEnter2D(Collider2D collision) {

        if (collision.TryGetComponent(out IMergable mergable)) {
            mergablesTouching.Add(mergable);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.TryGetComponent(out IMergable mergable)) {
            mergablesTouching.Remove(mergable);
        }
    }
}
