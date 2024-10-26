using System;
using UnityEngine;

public class StraightShootBehavior : MonoBehaviour, IAttacker {

    public event Action<Vector2> OnShoot_Direction;
    public event Action<GameObject> OnShoot_Projectile;
    public event Action OnAttack;

    public event Action OnShootAnim;

    protected IHasStats hasStats;

    [SerializeField] protected StraightMovement projectilePrefab;
    [SerializeField] protected Transform shootPoint;

    [SerializeField] private bool overrideDamage;
    [ConditionalHide("overrideDamage")][SerializeField] private float damage;

    [SerializeField] private bool hasShootVariation;
    [ConditionalHide("hasShootVariation")][SerializeField] private float shootVariation;

    [Header("Animation")]
    [SerializeField] private bool specialAttack;
    [SerializeField] private Animator enemyAnim;
    [SerializeField] private bool hasWeapon;
    [ConditionalHide("hasWeapon")][SerializeField] private Animator weaponAnim;

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
        Vector2 shootDirection = toTarget;

        if (hasShootVariation) {
            float randomAngle = UnityEngine.Random.Range(-shootVariation, shootVariation);
            shootDirection = shootDirection.RotateDirection(randomAngle);
        }

        newProjectile.Setup(shootDirection.normalized);

        float dmg = overrideDamage ? damage : hasStats.GetStats().Damage;
        newProjectile.GetComponent<DamageOnContact>().Setup(dmg, hasStats.GetStats().KnockbackStrength);

        InvokeShootDirectionEvent(shootDirection.normalized);
        InvokeShootProjectileEvent(newProjectile.gameObject);
        InvokeAttackEvent();
    }

    // methods needed for ShootStraightSpreadBehavior
    protected void InvokeShootDirectionEvent(Vector2 direction) {
        OnShoot_Direction?.Invoke(direction);
    }
    protected void InvokeShootProjectileEvent(GameObject projectile) {
        OnShoot_Projectile?.Invoke(projectile);
    }
    protected void InvokeAttackEvent() {
        OnAttack?.Invoke();
    }
}