using System;
using UnityEngine;

public class StraightShootBehavior : MonoBehaviour, ISpecialAttacker {

    public event Action OnShootAnim;
    public event Action<Vector2> OnShoot_Direction;
    public event Action OnSpecialAttack;

    private IHasStats hasStats;

    [SerializeField] protected StraightMovement projectilePrefab;
    [SerializeField] protected Transform shootPoint;

    protected Transform target;
    private TimedActionBehavior timedActionBehavior;

    private void Awake() {
        hasStats = GetComponent<IHasStats>();

        timedActionBehavior = new TimedActionBehavior(
            hasStats.GetStats().AttackCooldown,
            () => ShootAnimation()
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

    private void ShootAnimation() {
        enemy.InvokeAttack();
        OnShootAnim?.Invoke();
    }

    protected virtual void CreateProjectile() {
        StraightMovement newProjectile = projectilePrefab.Spawn(shootPoint.position, Containers.Instance.Projectiles);

        Vector2 toTarget = target.position - transform.position;
        newProjectile.Setup(toTarget.normalized);
        newProjectile.GetComponent<DamageOnContact>().Setup(hasStats.GetStats().Damage, hasStats.GetStats().KnockbackStrength);

        InvokeShoot(toTarget.normalized);
    }

    protected void InvokeShoot(Vector2 direction) {
        OnShoot_Direction?.Invoke(direction);
    }

    public override void DoAnimationTriggerEventLogic(AnimationTriggerType triggerType) {
        base.DoAnimationTriggerEventLogic(triggerType);

        if (triggerType == AnimationTriggerType.ShootStraight) {
            CreateProjectile();
        }
    }
}