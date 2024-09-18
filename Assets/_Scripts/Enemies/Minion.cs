using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Minion : Enemy, IMergable {

    private ChasePlayerBehavior moveBehavior;

    private MergeBehavior mergeBehavior;
    [SerializeField] private TriggerContactTracker mergeTracker;
    [SerializeField] private Enemy mergedEnemyPrefab;
    [SerializeField] private float toMergeDelay;
    [SerializeField] private float mergeTime;

    [Header("Split On Death")]
    [SerializeField] private Enemy splitEnemyPrefab;

    public GameObject GetObject() {
        return gameObject;
    }

    public MergeBehavior GetMergeBehavior() {
        return mergeBehavior;
    }

    protected override void Awake() {
        base.Awake();

        InitializeBehaviors();
    }

    private void InitializeBehaviors() {
        moveBehavior = new(this);
        enemyBehaviors.Add(moveBehavior);

        mergeBehavior = new(this, mergeTracker, mergedEnemyPrefab, toMergeDelay, mergeTime);
        if (mergedEnemyPrefab != null) {
            mergeBehavior.AllowMerging();
        }
        enemyBehaviors.Add(mergeBehavior);
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

        if (mergedEnemyPrefab != null) {
            mergeBehavior.AllowMerging();
        }
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

    public override void OnRemoveStopMovementEffect() {
        base.OnRemoveStopMovementEffect();

        if (!mergeBehavior.IsMovingToMerge() && !mergeBehavior.IsMerging()) {
            moveBehavior.Start();
        }
    }

    #region Split On Destroy

    protected override void Start() {
        base.Start();

        health.OnDeath += SpawnTwoMinions;
    }

    private void OnDestroy() {
        health.OnDeath -= SpawnTwoMinions;
    }

    private void SpawnTwoMinions() {

        if (splitEnemyPrefab == null) {
            return;
        }

        float offsetValue = 0.5f;

        Vector3 firstOffset = new(-offsetValue, 0);
        splitEnemyPrefab.Spawn(transform.position + firstOffset, Containers.Instance.Enemies);

        Vector3 secondOffset = new(offsetValue, 0);
        splitEnemyPrefab.Spawn(transform.position + secondOffset, Containers.Instance.Enemies);
    }

    #endregion
}
