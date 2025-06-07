using UnityEngine;

public class CircleStraightShootBehavior : MonoBehaviour {

    [SerializeField] private StraightMovement projectilePrefab;
    [SerializeField] private int projectileCount;
    [SerializeField] private float attackCooldownMult = 1f;
    [SerializeField] private float distanceFromCenter = 1f;

    [SerializeField] private bool alternateShootDirection;
    private bool thisShotIsAlternate;

    [SerializeField] private bool hasShootPoint;
    [SerializeField, ConditionalHide("hasShootPoint")] private Transform shootPoint;

    private IHasEnemyStats hasStats;

    private TimedActionBehavior timedActionBehavior;

    [Header("Animation")]
    [SerializeField] private Animator anim;
    [SerializeField] private bool specialAttack;

    [Header("SFX")]
    [SerializeField] private bool customSFX;
    [ConditionalHide("customSFX")][SerializeField] private AudioClips shootSFX;

    [SerializeField] private bool hasStartAnimSfx;
    [SerializeField, ConditionalHide("hasStartAnimSfx")] private AudioClips startAnimSfx;

    private void Awake() {
        hasStats = GetComponent<IHasEnemyStats>();

        timedActionBehavior = new TimedActionBehavior(
            hasStats.            EnemyStats.AttackCooldown * attackCooldownMult,
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
        anim.SetTrigger(attackTriggerString);

        if (hasStartAnimSfx) {
            AudioManager.Instance.PlaySingleSound(startAnimSfx);
        }
    }

    // played by animation
    public void CircleShoot() {

        // Calculate the angle between each shockwave
        float angleStep = 360f / projectileCount;
        float angle = 0f;

        if (alternateShootDirection) {
            if (thisShotIsAlternate) {
                angle = angleStep * 0.5f;
            }
            thisShotIsAlternate = !thisShotIsAlternate;
        }

        for (int i = 0; i < projectileCount; i++) {
            Vector2 projectileDirection = angle.RotationToDirection();

            Vector2 shootPos = hasShootPoint ? shootPoint.position : transform.position;
            Vector2 spawnPosition = shootPos + projectileDirection * distanceFromCenter;

            StraightMovement projectile = projectilePrefab.Spawn(spawnPosition, Containers.Instance.Projectiles);
            projectile.Setup(projectileDirection);
            projectile.GetComponent<DamageOnContact>().Setup(hasStats.EnemyStats.Damage, hasStats.EnemyStats.KnockbackStrength);
            projectile.transform.up = projectileDirection;

            angle += angleStep;
        }

        if (customSFX) {
            AudioManager.Instance.PlaySingleSound(shootSFX);
        }
        else {
            AudioManager.Instance.PlaySingleSound(AudioManager.Instance.AudioClips.BasicEnemyShoot);
        }
    }

    public float GetActionTimer() {
        return timedActionBehavior.GetActionTimer();
    }

    public void SetActionTimer(float timerValue) {
         timedActionBehavior.SetActionTimer(timerValue);
    }
}