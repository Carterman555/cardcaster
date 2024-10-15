using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using QFSW.QC;

public class Enemy : MonoBehaviour, IHasStats, IChangesFacing, IEffectable {

    #region Events

    public event Action<bool> OnChangedFacing;
    public void InvokeChangedFacing(bool facing) {
        OnChangedFacing?.Invoke(facing);
    }

    #endregion

    protected Health health;

    protected virtual void Awake() {
        health = GetComponent<Health>();
    }

    protected virtual void OnEnable() {
        SubToPlayerTriggerEvents();

        InvokeOnEnable();
    }

    protected virtual void OnDisable() {
        UnsubFromPlayerTriggerEvents();

        InvokeOnDisable();
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

    #region Handle Behaviors

    protected List<EnemyBehavior> enemyBehaviors = new();

    protected virtual void Start() {
        playerTracker.SetRange(stats.AttackRange);
    }

    public bool IsMoving() {
        foreach (EnemyBehavior behavior in enemyBehaviors.Where(b => b is IMovementBehavior)) {
            if (!behavior.IsStopped()) {
                return true;
            }
        }
        return false;
    }

    private void InvokeOnEnable() {
        foreach (EnemyBehavior behavior in enemyBehaviors) {
            behavior.OnEnable();
        }
    }

    private void InvokeOnDisable() {
        foreach (EnemyBehavior behavior in enemyBehaviors) {
            behavior.OnDisable();
        }
    }

    protected virtual void Update() {
        foreach (EnemyBehavior behavior in enemyBehaviors) {
            behavior.FrameUpdateLogic();
        }
    }

    protected virtual void FixedUpdate() {
        foreach (EnemyBehavior behavior in enemyBehaviors) {
            behavior.PhysicsUpdateLogic();
        }
    }

    public virtual void OnAddEffect(UnitEffect unitEffect) {

    }

    public void AnimationTriggerEvent(AnimationTriggerType triggerType) {
        foreach (EnemyBehavior behavior in enemyBehaviors) {
            behavior.DoAnimationTriggerEventLogic(triggerType);
        }
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

    // debugging
    [Command("kill_all", MonoTargetType.All)]
    [Command("kill", MonoTargetType.Argument)]
    private void KillEnemy() {
        health.Die();
    }

    
}