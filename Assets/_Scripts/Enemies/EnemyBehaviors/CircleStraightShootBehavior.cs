using Mono.CSharp;
using UnityEngine;

public class CircleStraightShootBehavior : MonoBehaviour {

    [SerializeField] private StraightMovement projectilePrefab;
    [SerializeField] private int projectileCount;
    [SerializeField] private float attackCooldownMult = 1f;

    [SerializeField] private bool alternateShootDirection;
    private bool thisShotIsAlternate;

    private IHasEnemyStats hasStats;

    private TimedActionBehavior timedActionBehavior;

    [Header("Animation")]
    [SerializeField] private Animator anim;
    [SerializeField] private bool specialAttack;

    [Header("SFX")]
    [SerializeField] private bool customSFX;
    [ConditionalHide("customSFX")][SerializeField] private AudioClips shootSFX;

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

            float distanceFromCenter = 1f;

            Vector2 spawnPosition = (Vector2)transform.position + projectileDirection * distanceFromCenter;
            StraightMovement projectile = projectilePrefab
                .Spawn(spawnPosition, Containers.Instance.Projectiles);
            projectile.Setup(projectileDirection);
            projectile.GetComponent<DamageOnContact>().Setup(hasStats.EnemyStats.Damage, hasStats.EnemyStats.KnockbackStrength);
            projectile.transform.up = projectileDirection;

            angle += angleStep;
        }

        if (customSFX) {
            AudioManager.Instance.PlaySound(shootSFX);
        }
        else {
            AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.BasicEnemyShoot);
        }
    }

    public float GetActionTimer() {
        return timedActionBehavior.GetActionTimer();
    }

    public void SetActionTimer(float timerValue) {
         timedActionBehavior.SetActionTimer(timerValue);
    }
}