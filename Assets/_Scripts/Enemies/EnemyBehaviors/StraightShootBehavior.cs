using System;
using UnityEngine;

public class StraightShootBehavior : MonoBehaviour, IAttacker {

    public event Action<Vector2> OnShoot_Direction;
    public event Action OnAttack;

    public event Action OnShootAnim;

    protected IHasStats hasStats;

    [SerializeField] protected StraightMovement projectilePrefab;
    [SerializeField] protected Transform shootPoint;

    [Header("Animation")]
    [SerializeField] private bool specialAttack;
    [SerializeField] private Animator enemyAnim;
    [SerializeField] private bool hasWeapon;
    [ConditionalHide("hasWeapon")] [SerializeField] private Animator weaponAnim;

    private TimedActionBehavior timedActionBehavior;

    private void Awake() {
        hasStats = GetComponent<IHasStats>();

        timedActionBehavior = new TimedActionBehavior(
            hasStats.GetStats().AttackCooldown,
            () => TriggerShootAnimation()
        );
    }

    private void OnEnable() {
        timedActionBehavior.Start();
    }
    private void OnDisable() {
        timedActionBehavior.Stop();
    }

    private void Update() {
        timedActionBehavior.UpdateLogic();
    }

    private void TriggerShootAnimation() {
        string attackTriggerString = specialAttack ? "specialAttack" : "attack";
        enemyAnim.SetTrigger(attackTriggerString);

        if (hasWeapon) {
            weaponAnim.SetTrigger("shoot");
        }

        OnShootAnim?.Invoke();
    }

    public virtual void ShootProjectile() {
        StraightMovement newProjectile = projectilePrefab.Spawn(shootPoint.position, Containers.Instance.Projectiles);

        Vector2 toTarget = PlayerMeleeAttack.Instance.transform.position - transform.position;
        newProjectile.Setup(toTarget.normalized);
        newProjectile.GetComponent<DamageOnContact>().Setup(hasStats.GetStats().Damage, hasStats.GetStats().KnockbackStrength);

        InvokeEvents(toTarget.normalized);
    }

    protected void InvokeEvents(Vector2 direction) {
        OnShoot_Direction?.Invoke(direction);
        OnAttack?.Invoke();
    }
}