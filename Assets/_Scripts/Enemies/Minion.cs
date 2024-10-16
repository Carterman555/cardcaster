using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Minion : Enemy {

    private ChasePlayerBehavior moveBehavior;

    [Header("Attack")]
    private CircleSlashBehavior attackBehavior;
    [SerializeField] private Transform attackCenter;

    private MergeBehavior mergeBehavior;

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

        mergeBehavior = GetComponent<MergeBehavior>();

        InitializeBehaviors();
    }

    protected override void Start() {
        base.Start();

        health.OnDeath += OnDeath;
    }

    private void OnDestroy() {
        health.OnDeath -= OnDeath;
    }

    protected override void Update() {
        base.Update();

        // stop chasing the player when merging
        if (mergeBehavior.IsMerging()) {
            if (moveBehavior.enabled) {
                moveBehavior.enabled = false;
            }
        }
        else {
            if (!moveBehavior.enabled) {
                moveBehavior.enabled = true;
            }
        }

        HandleMergeIndicator();
    }

    // only merges when not close to player
    protected override void OnPlayerEnteredRange(GameObject player) {
        base.OnPlayerEnteredRange(player);

        if (!mergeBehavior.IsMerging()) {
            moveBehavior.enabled = false;
            attackBehavior.Start();
            mergeBehavior.DontAllowMerging();
        }
    }

    protected override void OnPlayerExitedRange(GameObject player) {
        base.OnPlayerExitedRange(player);

        moveBehavior.enabled = true;
        attackBehavior.Stop();
        mergeBehavior.AllowMerging();
    }

    public override void OnAddEffect(UnitEffect unitEffect) {
        base.OnAddEffect(unitEffect);

        if (unitEffect is StopMovement) {
            if (!mergeBehavior.IsMerging()) {
                moveBehavior.enabled = true;
            }
        }
    }

    private void OnDeath() {

        if (mergeBehavior.IsMerging()) {
            mergeBehavior.StopMerging();
        }

        SpawnTwoMinions();
    }

    

    #region Split On Destroy

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
