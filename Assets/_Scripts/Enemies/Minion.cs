using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Minion : Enemy {

    private ChasePlayerBehavior moveBehavior;
    private CircleSlashBehavior attackBehavior;
    private MergeBehavior mergeBehavior;

    [SerializeField] private bool isMergable;

    [SerializeField] private bool splitOnDeath;
    [ConditionalHide("splitOnDeath")] [SerializeField] private Enemy splitEnemyPrefab;

    protected override void Awake() {
        base.Awake();

        moveBehavior = GetComponent<ChasePlayerBehavior>();
        attackBehavior = GetComponent<CircleSlashBehavior>();

        if (isMergable) {
            mergeBehavior = GetComponent<MergeBehavior>();
            mergeBehavior.AllowMerging();
        }

        moveBehavior.enabled = true;
        attackBehavior.enabled = false;
    }

    private void Start() {
        health.OnDeath += OnDeath;
    }

    private void OnDestroy() {
        health.OnDeath -= OnDeath;
    }

    protected override void Update() {
        base.Update();

        // stop chasing the player when merging
        if (isMergable && mergeBehavior.IsMerging()) {
            if (moveBehavior.enabled) {
                moveBehavior.enabled = false;
            }
        }
        else {
            if (!moveBehavior.enabled) {
                moveBehavior.enabled = true;
            }
        }
    }

    // only merges when not close to player
    protected override void OnPlayerEnteredRange(GameObject player) {
        base.OnPlayerEnteredRange(player);

        if (!isMergable || !mergeBehavior.IsMerging()) {
            moveBehavior.enabled = false;
            attackBehavior.enabled = true;

            if (isMergable) {
                mergeBehavior.DontAllowMerging();
            }
        }
    }

    protected override void OnPlayerExitedRange(GameObject player) {
        base.OnPlayerExitedRange(player);

        moveBehavior.enabled = true;
        attackBehavior.enabled = false;

        if (isMergable) {
            mergeBehavior.AllowMerging();
        }
    }

    public override void OnAddEffect(UnitEffect unitEffect) {
        base.OnAddEffect(unitEffect);

        if (unitEffect is StopMovement) {
            moveBehavior.enabled = false;
        }
    }

    private void OnDeath() {

        if (isMergable && mergeBehavior.IsMerging()) {
            mergeBehavior.StopMerging();
        }

        SpawnTwoMinions();
    }

    #region Split On Destroy

    private void SpawnTwoMinions() {

        if (!splitOnDeath) {
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
