using Mono.CSharp;
using UnityEngine;

public class CircleStraightShootBehavior : MonoBehaviour {

    [SerializeField] private StraightMovement projectilePrefab;
    [SerializeField] private int projectileCount;
    [SerializeField] private float attackCooldownMult = 1f;

    private IHasStats hasStats;

    private TimedActionBehavior timedActionBehavior;

    [Header("Animation")]
    [SerializeField] private Animator anim;
    [SerializeField] private bool specialAttack;


    private void Awake() {
        hasStats = GetComponent<IHasStats>();

        timedActionBehavior = new TimedActionBehavior(
            hasStats.GetStats().AttackCooldown * attackCooldownMult,
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
        for (int i = 0; i < projectileCount; i++) {
            Vector2 projectileDirection = angle.RotationToDirection();

            float distanceFromCenter = 1f;

            Vector2 spawnPosition = (Vector2)transform.position + projectileDirection * distanceFromCenter;
            StraightMovement projectile = projectilePrefab
                .Spawn(spawnPosition, Containers.Instance.Projectiles);
            projectile.Setup(projectileDirection);
            projectile.GetComponent<DamageOnContact>().Setup(hasStats.GetStats().Damage, hasStats.GetStats().KnockbackStrength);
            projectile.transform.up = projectileDirection;

            angle += angleStep;
        }
    }

    public float GetActionTimer() {
        return timedActionBehavior.GetActionTimer();
    }

    public void SetActionTimer(float timerValue) {
         timedActionBehavior.SetActionTimer(timerValue);
    }
}