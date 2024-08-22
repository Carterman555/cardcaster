using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IHasStats {

    [SerializeField] protected ScriptableEnemy scriptableEnemy;
    protected EnemyStats stats => scriptableEnemy.Stats;
    public Stats GetStats() {
        return stats;
    }

    [SerializeField] private TriggerContactTracker playerTracker;
    protected bool playerWithinRange => playerTracker.HasContact();

    protected List<EnemyBehavior> enemyBehaviors = new();

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