using System;
using UnityEngine;

public class ShootTargetProjectileBehavior : MonoBehaviour {
    public event Action OnShoot;

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootPoint;

    [Header("Animation")]
    [SerializeField] private bool specialAttack;
    [SerializeField] private Animator anim;
    private IHasStats hasStats;

    private TimedActionBehavior timedActionBehavior;

    private void Awake() {

        hasStats = GetComponent<IHasStats>();

        timedActionBehavior = new TimedActionBehavior(
            hasStats.GetStats().AttackCooldown,
            () => TriggerShootAnimation()
        );

        enabled = false;
    }

    public void StartShooting(int amountToShoot) {
        timedActionBehavior.Start(amountToShoot);
    }

    private void OnDisable() {
        timedActionBehavior.Stop();
    }

    private void Update() {
        timedActionBehavior.UpdateLogic();
        if (timedActionBehavior.IsFinished()) {
           enabled = true;
        }
    }

    private void TriggerShootAnimation() {
        string attackTriggerString = specialAttack ? "specialAttack" : "attack";
        anim.SetTrigger(attackTriggerString);
    }

    // played by animation
    private void ShootProjectile() {
        GameObject newProjectileObject = projectilePrefab.Spawn(shootPoint.position, Containers.Instance.Enemies);
        ITargetMovement newProjectile = newProjectileObject.GetComponent<ITargetMovement>();
        newProjectile.Setup(PlayerMovement.Instance.transform);
        newProjectileObject.GetComponent<DamageOnContact>().Setup(hasStats.GetStats().Damage, hasStats.GetStats().KnockbackStrength);

        OnShoot?.Invoke();
    }
}