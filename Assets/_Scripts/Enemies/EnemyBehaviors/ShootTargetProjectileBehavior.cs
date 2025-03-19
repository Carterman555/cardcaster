using System;
using UnityEngine;

public class ShootTargetProjectileBehavior : MonoBehaviour {
    public event Action OnShoot;

    [SerializeField] private RandomInt amountToShoot;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootPoint;

    [Header("Animation")]
    [SerializeField] private bool specialAttack;
    [SerializeField] private Animator anim;
    private IHasStats hasStats;

    private TimedActionBehavior timedActionBehavior;

    [Header("SFX")]
    [SerializeField] private bool customSFX;
    [ConditionalHide("customSFX")][SerializeField] private AudioClips shootSFX;

    private void Awake() {

        hasStats = GetComponent<IHasStats>();

        timedActionBehavior = new TimedActionBehavior(
            hasStats.            Stats.AttackCooldown,
            () => TriggerShootAnimation()
        );
    }

    private void OnEnable() {
        timedActionBehavior.Start(amountToShoot.Randomize());
    }

    private void OnDisable() {
        timedActionBehavior.Stop();
    }

    private void Update() {
        timedActionBehavior.UpdateLogic();
        if (timedActionBehavior.IsFinished()) {
           enabled = false;
        }
    }

    private void TriggerShootAnimation() {
        string attackTriggerString = specialAttack ? "specialAttack" : "attack";
        anim.SetTrigger(attackTriggerString);
    }

    // played by animation
    public void ShootProjectile() {
        GameObject newProjectileObject = projectilePrefab.Spawn(shootPoint.position, Containers.Instance.Enemies);
        ITargetProjectileMovement newProjectile = newProjectileObject.GetComponent<ITargetProjectileMovement>();
        newProjectile.Setup(PlayerMovement.Instance.transform);
        newProjectileObject.GetComponent<DamageOnContact>().Setup(hasStats.Stats.Damage, hasStats.Stats.KnockbackStrength);

        if (customSFX) {
            AudioManager.Instance.PlaySound(shootSFX);
        }
        else {
            AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.BasicEnemyShoot);
        }

        OnShoot?.Invoke();
    }
}