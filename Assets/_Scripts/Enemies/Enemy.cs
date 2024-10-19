using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using QFSW.QC;

public class Enemy : MonoBehaviour, IHasEnemyStats, IEffectable {

    protected Health health;

    protected virtual void Awake() {
        health = GetComponent<Health>();
        moveBehaviours = GetComponents<IEnemyMovement>();
    }

    protected virtual void OnEnable() {
        SubToPlayerTriggerEvents();
    }

    protected virtual void OnDisable() {
        UnsubFromPlayerTriggerEvents();
        OnPlayerExitedRange(PlayerMovement.Instance.gameObject);
    }

    protected virtual void Update() {
        HandleMoveAnim();
    }

    #region Stats

    [SerializeField] protected ScriptableEnemy scriptableEnemy;
    protected EnemyStats stats => scriptableEnemy.Stats;
    public Stats GetStats() {
        return stats;
    }
    public EnemyStats GetEnemyStats() {
        return stats;
    }

    #endregion

    #region Player Tracker

    [SerializeField] private TriggerContactTracker playerTracker;
    protected bool playerWithinRange => playerTracker.HasContact();

    private void SubToPlayerTriggerEvents() {
        playerTracker.OnEnterContact += OnPlayerEnteredRange;
        playerTracker.OnExitContact += OnPlayerExitedRange;
    }
    private void UnsubFromPlayerTriggerEvents() {
        playerTracker.OnEnterContact -= OnPlayerEnteredRange;
        playerTracker.OnExitContact -= OnPlayerExitedRange;
    }

    protected virtual void OnPlayerEnteredRange(GameObject player) {

    }

    protected virtual void OnPlayerExitedRange(GameObject player) {

    }

    #endregion

    public virtual void OnAddEffect(UnitEffect unitEffect) {
    }

    #region Animation

    [SerializeField] private Animator anim;

    private IEnemyMovement[] moveBehaviours;

    private void HandleMoveAnim() {
        bool isMoving = moveBehaviours.Any(m => m.IsMoving());
        anim.SetBool("move", isMoving);
    }

    #endregion

    // debugging
    [Command("kill_all", MonoTargetType.All)]
    [Command("kill", MonoTargetType.Argument)]
    private void KillEnemy() {
        health.Die();
    }
}