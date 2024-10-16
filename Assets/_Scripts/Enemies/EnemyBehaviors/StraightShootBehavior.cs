using System;
using UnityEngine;

public class StraightShootBehavior : MonoBehaviour, IAttacker {

    public event Action<Vector2> OnShoot_Direction;
    public event Action OnAttack;

    protected IHasStats hasStats;

    [SerializeField] private bool specialAttack;

    [SerializeField] protected StraightMovement projectilePrefab;
    [SerializeField] protected Transform shootPoint;

    [Header("Animators")]
    [SerializeField] private Animator enemyAnim;
    [SerializeField] private Animator weaponAnim;

    protected Transform target;
    private TimedActionBehavior timedActionBehavior;

    private void Awake() {
        hasStats = GetComponent<IHasStats>();

        timedActionBehavior = new TimedActionBehavior(
            hasStats.GetStats().AttackCooldown,
            () => TriggerShootAnimation()
        );
    }

    private void Update() {
        timedActionBehavior.UpdateLogic();
    }

    public void StartShooting(Transform target) {
        this.target = target;
        timedActionBehavior.Start();

        timedActionBehavior.SetActionCooldown(hasStats.GetStats().AttackCooldown);
    }

    private void TriggerShootAnimation() {
        string attackTriggerString = specialAttack ? "specialAttack" : "attack";
        enemyAnim.SetTrigger(attackTriggerString);
        weaponAnim.SetTrigger("shoot");
    }

    protected virtual void ShootProjectile() {
        StraightMovement newProjectile = projectilePrefab.Spawn(shootPoint.position, Containers.Instance.Projectiles);

        Vector2 toTarget = target.position - transform.position;
        newProjectile.Setup(toTarget.normalized);
        newProjectile.GetComponent<DamageOnContact>().Setup(hasStats.GetStats().Damage, hasStats.GetStats().KnockbackStrength);

        InvokeEvents(toTarget.normalized);
    }

    protected void InvokeEvents(Vector2 direction) {
        OnShoot_Direction?.Invoke(direction);
        OnAttack?.Invoke();
    }
}