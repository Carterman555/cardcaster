using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using QFSW.QC;

public class Enemy : MonoBehaviour, IHasEnemyStats, IEffectable {

    public static event Action<Enemy> OnAnySpawn;

    protected Health health;

    protected virtual void Awake() {
        health = GetComponent<Health>();
        moveBehaviours = GetComponents<IEnemyMovement>();
    }

    protected virtual void OnEnable() {
        SubToPlayerTriggerEvents();
        playerTracker.GetComponent<CircleCollider2D>().radius = stats.AttackRange;
        OnAnySpawn?.Invoke(this);
    }

    protected virtual void OnDisable() {
        UnsubFromPlayerTriggerEvents();

        if (Helpers.GameStopping()) {
            return;
        }

        OnPlayerExitedRange(PlayerMovement.Instance.gameObject);
    }

    protected virtual void Update() {
        HandleMoveAnim();
    }

    #region Stats

    [SerializeField] protected ScriptableEnemy scriptableEnemy;
    protected EnemyStats stats => scriptableEnemy.Stats;
    public Stats Stats => stats;
    public EnemyStats GetEnemyStats() {
        return stats;
    }

    #endregion

    #region Player Tracker

    [SerializeField] private TriggerContactTracker playerTracker;
    protected bool playerWithinRange => playerTracker.HasContact();

    private void SubToPlayerTriggerEvents() {
        playerTracker.OnEnterContact_GO += OnPlayerEnteredRange;
        playerTracker.OnExitContact_GO += OnPlayerExitedRange;
    }
    private void UnsubFromPlayerTriggerEvents() {
        playerTracker.OnEnterContact_GO -= OnPlayerEnteredRange;
        playerTracker.OnExitContact_GO -= OnPlayerExitedRange;
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