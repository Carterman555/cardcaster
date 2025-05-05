using System;
using UnityEngine;

public class StraightShootBehavior : MonoBehaviour {

    public event Action<Vector2> OnShoot_Direction;
    public event Action<GameObject> OnShoot_Projectile;

    public event Action OnShootAnim;

    protected IHasEnemyStats hasStats;

    [SerializeField] protected StraightMovement projectilePrefab;
    [SerializeField] protected Transform shootPoint;

    [SerializeField] private bool overrideDamage;
    [ConditionalHide("overrideDamage")][SerializeField] private float damage;

    [SerializeField] private bool hasShootVariation;
    [ConditionalHide("hasShootVariation")][SerializeField] private float shootVariation;

    public enum ShootTarget { Player, Random }
    private ShootTarget shootTarget = ShootTarget.Player;

    [Header("Animation")]
    [SerializeField] private bool hasShootAnim = true;
    [ConditionalHide("hasShootAnim")][SerializeField] private bool specialAttack;
    [ConditionalHide("hasShootAnim")][SerializeField] private bool stopAimingOnAnimStart;
    [ConditionalHide("hasShootAnim")][SerializeField] private Animator enemyAnim;
    [SerializeField] private bool hasWeapon;
    [ConditionalHide("hasWeapon")][SerializeField] private Animator weaponAnim;

    [Header("SFX")]
    [SerializeField] private bool customSFX;
    [ConditionalHide("customSFX")][SerializeField] private AudioClips shootSFX;

    private TimedActionBehavior timedActionBehavior;

    private void Awake() {
        hasStats = GetComponent<IHasEnemyStats>();

        if (hasShootAnim) {
            timedActionBehavior = new TimedActionBehavior(
            hasStats.EnemyStats.AttackCooldown,
            () => TriggerShootAnimation());
        }
        else {
            timedActionBehavior = new TimedActionBehavior(
            hasStats.EnemyStats.AttackCooldown,
            () => ShootProjectile());
        }
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

    private Vector2 shootDirection;

    public void TriggerShootAnimation() {
        string attackTriggerString = specialAttack ? "specialAttack" : "attack";
        enemyAnim.SetTrigger(attackTriggerString);

        if (hasWeapon) {
            weaponAnim.SetTrigger("shoot");
        }

        OnShootAnim?.Invoke();

        // save direction to player when start animation to stop aiming after animation has started
        if (stopAimingOnAnimStart) {
            shootDirection = GetShootDirection();
            if (weaponAnim.TryGetComponent(out PointTowardsPlayer pointTowardsPlayer)) {
                pointTowardsPlayer.enabled = false;
            }
        }
    }

    public virtual void ShootProjectile() {
        StraightMovement newProjectile = projectilePrefab.Spawn(shootPoint.position, Containers.Instance.Projectiles);

        // the weapon stopped pointing towards player when animation started so make it point towards player again
        if (stopAimingOnAnimStart) {
            if (weaponAnim.TryGetComponent(out PointTowardsPlayer pointTowardsPlayer)) {
                pointTowardsPlayer.enabled = true;
            }
        }

        if (!stopAimingOnAnimStart) {
            shootDirection = GetShootDirection();
        }

        if (hasShootVariation) {
            float randomAngle = UnityEngine.Random.Range(-shootVariation, shootVariation);
            shootDirection.RotateDirection(randomAngle);
        }

        newProjectile.Setup(shootDirection.normalized);

        float dmg = overrideDamage ? damage : hasStats.EnemyStats.Damage;
        newProjectile.GetComponent<DamageOnContact>().Setup(dmg, hasStats.EnemyStats.KnockbackStrength);

        PlaySFX();

        InvokeShootDirectionEvent(shootDirection.normalized);
        InvokeShootProjectileEvent(newProjectile.gameObject);
    }

    protected void PlaySFX() {
        if (customSFX) {
            AudioManager.Instance.PlaySound(shootSFX);
        }
        else {
            AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.BasicEnemyShoot);
        }
    }

    // methods needed for ShootStraightSpreadBehavior
    protected void InvokeShootDirectionEvent(Vector2 direction) {
        OnShoot_Direction?.Invoke(direction);
    }
    protected void InvokeShootProjectileEvent(GameObject projectile) {
        OnShoot_Projectile?.Invoke(projectile);
    }

    public void SetShootTarget(ShootTarget shootTarget) {
        this.shootTarget = shootTarget;
    }

    public Vector2 GetShootDirection() {
        if (shootTarget == ShootTarget.Player) {
            return PlayerDirection;
        }
        else if (shootTarget == ShootTarget.Random) {
            return RandomDirection;
        }
        else {
            Debug.LogError("Shoot Type Not Supported: " + shootTarget.ToString());
            return Vector2.zero;
        }
    }

    public Vector2 PlayerDirection => PlayerMovement.Instance.CenterPos - transform.position;

    public Vector2 RandomDirection => UnityEngine.Random.insideUnitCircle.normalized;
}