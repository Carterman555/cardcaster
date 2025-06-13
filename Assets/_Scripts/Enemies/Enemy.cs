using System;
using System.Linq;
using UnityEngine;

public class Enemy : MonoBehaviour, IHasEnemyStats, IEffectable {

    public static event Action<Enemy> OnAnySpawn;

    protected EnemyHealth health;

    //... either spawned in from spawn behavior or merged/split from a minion who was
    //... so no matter who many times minions merge/split the player can't farm essence
    //... from the witch spawning minions
    public bool FromSpawnBehavior { get; private set; }
    
    [SerializeField] protected ScriptableEnemy scriptableEnemy;
    public EnemyStats GetEnemyStats() {
        return StatsManager.GetScaledEnemyStats(scriptableEnemy.Stats);
    }

    [SerializeField] private TriggerEventInvoker playerTracker;
    protected bool playerWithinRange;
    protected bool PlayerInvincible => PlayerMovement.Instance.gameObject.layer == GameLayers.InvinciblePlayerLayer;

    protected virtual void Awake() {
        health = GetComponent<EnemyHealth>();
        moveBehaviours = GetComponents<IEnemyMovement>();
    }

    protected virtual void OnEnable() {
        playerTracker.OnTriggerEnter_Col += OnPlayerEnteredRange;
        playerTracker.OnTriggerExit_Col += OnPlayerExitedRange;

        playerTracker.GetComponent<CircleCollider2D>().radius = GetEnemyStats().AttackRange;
        OnAnySpawn?.Invoke(this);

        SetFromSpawnBehavior(false);
    }

    protected virtual void OnDisable() {
        playerTracker.OnTriggerEnter_Col -= OnPlayerEnteredRange;
        playerTracker.OnTriggerExit_Col -= OnPlayerExitedRange;

        if (Helpers.GameStopping()) {
            return;
        }

        OnPlayerExitedRange(PlayerMovement.Instance.GetComponent<Collider2D>());
    }

    protected virtual void Update() {
        HandleMoveAnim();
    }

    public void SetFromSpawnBehavior(bool value) {
        FromSpawnBehavior = value;

        // so player can't farm infinite essence. also need to reset newly spawned minions which
        // had drop essence disabled because I'm using spawning pool
        if (TryGetComponent(out DropEssenceOnDeath dropEssenceOnDeath)) {
            dropEssenceOnDeath.IsEnabled = !FromSpawnBehavior;
        }
    }

    protected virtual void OnPlayerEnteredRange(Collider2D playerCol) {
        playerWithinRange = true;
    }

    protected virtual void OnPlayerExitedRange(Collider2D playerCol) {
        playerWithinRange = false;
    }

    public virtual void OnAddEffect(UnitEffect unitEffect) {
    }

    #region Animation

    [SerializeField] protected Animator anim;

    private IEnemyMovement[] moveBehaviours;

    private void HandleMoveAnim() {
        bool isMoving = moveBehaviours.Any(m => m.IsMoving());
        anim.SetBool("move", isMoving);
    }

    #endregion
}