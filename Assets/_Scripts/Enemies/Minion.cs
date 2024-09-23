using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Minion : Enemy, IMergable {

    private ChasePlayerBehavior moveBehavior;

    [Header("Attack")]
    private CircleSlashBehavior attackBehavior;
    [SerializeField] private Transform attackCenter;

    [Header("Merging")]
    private MergeBehavior mergeBehavior;
    [SerializeField] private TriggerContactTracker mergeTracker;
    [SerializeField] private Enemy mergedEnemyPrefab;
    [SerializeField] private float toMergeDelay;
    [SerializeField] private float mergeTime;

    [Header("Merging Indicator")]
    [SerializeField] private FillController mergeIndicatorPrefab;
    private FillController mergeIndicator;
    private bool isHandlingIndicator;

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

    protected override void OnEnable() {
        base.OnEnable();
        mergeBehavior.OnLeaderMerged += DestroyMergingIndicator;
    }

    protected override void OnDisable() {
        base.OnDisable();
        mergeBehavior.OnLeaderMerged -= DestroyMergingIndicator;
    }

    private void InitializeBehaviors() {
        moveBehavior = new(this);
        enemyBehaviors.Add(moveBehavior);

        attackBehavior = new(this, attackCenter);
        enemyBehaviors.Add(attackBehavior);

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

        HandleMergeIndicator();
    }

    // only merges when not close to player
    protected override void OnPlayerEnteredRange(GameObject player) {
        base.OnPlayerEnteredRange(player);

        moveBehavior.Stop();
        attackBehavior.Start();
        mergeBehavior.DontAllowMerging();
    }

    protected override void OnPlayerExitedRange(GameObject player) {
        base.OnPlayerExitedRange(player);

        moveBehavior.Start();
        attackBehavior.Stop();

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

    public override void OnAddEffect(UnitEffect unitEffect) {
        base.OnAddEffect(unitEffect);

        if (unitEffect is StopMovement) {
            if (!mergeBehavior.IsMovingToMerge() && !mergeBehavior.IsMerging()) {
                moveBehavior.Start();
            }
        }
    }

    #region Handle Merge Indicator

    /// <summary>
    /// the merge leader creates an indicator to show the progress of the merging
    /// </summary>
    private void HandleMergeIndicator() {

        // only the merge leader handles the indicator so there aren't two
        if (!mergeBehavior.IsMergeLeader()) {
            return;
        }

        if (!isHandlingIndicator && mergeBehavior.IsMerging()) {
            isHandlingIndicator = true;

            Vector3 offset = new Vector3(0, 1.5f);
            mergeIndicator = mergeIndicatorPrefab.Spawn(transform.position + offset, Containers.Instance.WorldUI);
            mergeIndicator.SetFillAmount(0);
        }
        else if (isHandlingIndicator) {
            mergeIndicator.SetFillAmount(mergeBehavior.GetMergeProgress());
        }
    }

    // destroy the indicator when the merging is complete
    private void DestroyMergingIndicator() {
        mergeIndicator.gameObject.ReturnToPool();
    }

    #endregion

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
