using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using QFSW.QC;

public class Enemy : MonoBehaviour, IHasStats, IChangesFacing, ICanAttack {

    public static event Action OnEnemiesCleared;

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
    public EnemyStats GetEnemyStats() {
        return stats;
    }

    protected List<EnemyBehavior> enemyBehaviors = new();

    protected Health health;

    protected virtual void Awake() {
        health = GetComponent<Health>();
    }

    protected virtual void OnEnable() {
        SubToPlayerTriggerEvents();
    }

    protected virtual void OnDisable() {
        UnsubFromPlayerTriggerEvents();

        foreach (EnemyBehavior behavior in enemyBehaviors) {
            behavior.OnDisable();
        }
    }

    protected virtual void Start() {
        playerTracker.SetRange(stats.AttackRange);

        health.OnDeath += OnDeath;
    }

    protected virtual void OnDestroy() {
        health.OnDeath -= OnDeath;
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

    private void OnDeath() {
        CheckIfEnemiesCleared();

        DeckManager.Instance.IncreaseEssence(1f);
    }

    private void CheckIfEnemiesCleared() {
        bool anyAliveEnemies = Containers.Instance.Enemies.GetComponentsInChildren<Health>().Any(health => !health.IsDead());
        if (!anyAliveEnemies) {
            OnEnemiesCleared?.Invoke();
        }
    }

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

public enum AnimationTriggerType {
    Die,
    MeleeAttack,
    RangedAttack,
    Pickup,
}