using System;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IHasStats, IChangesFacing, ICanAttack {

    public event Action<bool> OnChangedFacing;
    public void InvokeChangedFacing(bool facing) {
        OnChangedFacing?.Invoke(facing);
    }

    public event Action OnAttack;
    public void InvokeAttack() {
        OnAttack?.Invoke();
    }

    [SerializeField] protected ScriptableEnemy scriptableEnemy;
    protected EnemyStats stats => scriptableEnemy.Stats;
    public Stats GetStats() {
        return stats;
    }

    protected List<EnemyBehavior> enemyBehaviors = new();

    #region Player Tracker

    [SerializeField] private TriggerContactTracker playerTracker;
    protected bool playerWithinRange => playerTracker.HasContact();

    protected virtual void OnEnable() {
        playerTracker.OnEnterContact += OnPlayerEnteredRange;
        playerTracker.OnLeaveContact += OnPlayerExitedRange;
    }

    protected virtual void OnDisable() {
        playerTracker.OnEnterContact -= OnPlayerEnteredRange;
        playerTracker.OnLeaveContact -= OnPlayerExitedRange;

        foreach (EnemyBehavior behavior in enemyBehaviors) {
            behavior.OnDisable();
        }
    }

    protected virtual void OnPlayerEnteredRange(GameObject player) {
        
    }

    protected virtual void OnPlayerExitedRange(GameObject player) {

    }

    #endregion

    private void Start() {
        playerTracker.SetRange(stats.AttackRange);
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

    protected virtual void AnimationTriggerEvent(AnimationTriggerType triggerType) { }

    
}

public enum AnimationTriggerType {
    Die,
    MeleeAttack,
    RangedAttack,
    Pickup,
}