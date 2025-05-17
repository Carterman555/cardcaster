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
    public EnemyStats EnemyStats => scriptableEnemy.Stats;

    [SerializeField] private TriggerContactTracker playerTracker;
    protected bool PlayerWithinRange => playerTracker.HasContact();

    protected virtual void Awake() {
        health = GetComponent<EnemyHealth>();
        moveBehaviours = GetComponents<IEnemyMovement>();
    }

    protected virtual void OnEnable() {
        playerTracker.OnEnterContact_GO += OnPlayerEnteredRange;
        playerTracker.OnExitContact_GO += OnPlayerExitedRange;

        playerTracker.GetComponent<CircleCollider2D>().radius = EnemyStats.AttackRange;
        OnAnySpawn?.Invoke(this);

        SetFromSpawnBehavior(false);
    }

    protected virtual void OnDisable() {
        playerTracker.OnEnterContact_GO -= OnPlayerEnteredRange;
        playerTracker.OnExitContact_GO -= OnPlayerExitedRange;

        if (Helpers.GameStopping()) {
            return;
        }

        OnPlayerExitedRange(PlayerMovement.Instance.gameObject);
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

    protected virtual void OnPlayerEnteredRange(GameObject player) {

    }

    protected virtual void OnPlayerExitedRange(GameObject player) {

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