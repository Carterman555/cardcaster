using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using QFSW.QC;

public class Enemy : MonoBehaviour, IHasStats, IChangesFacing, IAttacker, IEffectable {

    #region Events

    public static event Action OnEnemiesCleared;

    public event Action<bool> OnChangedFacing;
    public void InvokeChangedFacing(bool facing) {
        OnChangedFacing?.Invoke(facing);
    }

    public event Action OnAttack;
    public void InvokeAttack() {
        OnAttack?.Invoke();
    }

    #endregion

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

    #region Effects

    private List<UnitEffectBase> effects = new();

    public virtual void AddEffect(UnitEffectBase effect, bool removeAfterDuration = false, float duration = 0) {
        effects.Add(effect);
        effect.OnEffectAdded(this, removeAfterDuration, duration);

        if (effect is StopMovement) {
            OnAddStopMovementEffect();
        }
    }

    public virtual void RemoveEffect(UnitEffectBase effect) {

        UnitEffectBase effectToRemove = effects.FirstOrDefault(e => e.GetType().Equals(effect.GetType()));
        if (effectToRemove == null) {
            Debug.LogError("Does Not Have Effect Trying To Remove!");
            return;
        }

        effectToRemove.OnEffectRemoved();
        effects.Remove(effectToRemove);

        if (effectToRemove is StopMovement && !MovementStopped) {
            OnRemoveStopMovementEffect();
        }
    }

    public bool ContainsEffect(UnitEffectBase effect) {
        return effects.Any(e => e.GetType().Equals(effect.GetType()));
    }

    public virtual void OnAddStopMovementEffect() {
        foreach (EnemyBehavior behavior in enemyBehaviors) {
            if (behavior is IMovementBehavior) {
                behavior.Stop();
            }
        }
    }

    public virtual void OnRemoveStopMovementEffect() {
        foreach (EnemyBehavior behavior in enemyBehaviors) {
            if (behavior is IMovementBehavior) {
                behavior.Start();
            }
        }
    }

    public bool MovementStopped => ContainsEffect(new StopMovement());

    #endregion

    #region Handle Behaviors

    protected List<EnemyBehavior> enemyBehaviors = new();

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

    public bool IsMoving() {
        foreach (EnemyBehavior behavior in enemyBehaviors.Where(b => b is IMovementBehavior)) {
            if (!behavior.IsStopped()) {
                return true;
            }
        }
        return false;
    }


    protected virtual void Update() {
        foreach (EnemyBehavior behavior in enemyBehaviors) {
            behavior.FrameUpdateLogic();
        }

        // not foreach because update logic may remove effect from effects list
        for (int i = effects.Count - 1; i >= 0; i--) {
            effects[i].UpdateLogic();
        }
    }

    protected virtual void FixedUpdate() {
        foreach (EnemyBehavior behavior in enemyBehaviors) {
            behavior.PhysicsUpdateLogic();
        }
    }

    public void AnimationTriggerEvent(AnimationTriggerType triggerType) {
        foreach (EnemyBehavior behavior in enemyBehaviors) {
            behavior.DoAnimationTriggerEventLogic(triggerType);
        }
    }

    #endregion

    protected Health health;

    protected virtual void Awake() {
        health = GetComponent<Health>();
    }

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