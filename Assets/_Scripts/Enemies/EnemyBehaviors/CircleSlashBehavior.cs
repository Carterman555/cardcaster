using UnityEngine;

public class CircleSlashBehavior : MonoBehaviour {

    private TimedActionBehavior timedActionBehavior;
    [SerializeField] private Transform centerPoint;

    private IHasEnemyStats hasStats;

    [Header("Animation")]
    [SerializeField] private bool specialAttack;
    [SerializeField] private Animator anim;

    private void Awake() {
        hasStats = GetComponent<IHasEnemyStats>();

        timedActionBehavior = new TimedActionBehavior(
            hasStats.EnemyStats.AttackCooldown,
            () => TriggerAttackAnimation()
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

    private void TriggerAttackAnimation() {
        string attackTriggerString = specialAttack ? "specialAttack" : "attack";
        anim.SetTrigger(attackTriggerString);
    }

    // played by animation
    public void Attack() {
        DamageDealer.DealCircleDamage(
            GameLayers.PlayerLayerMask,
            centerPoint.position,
            hasStats.EnemyStats.AttackRange,
            hasStats.EnemyStats.Damage,
            hasStats.EnemyStats.KnockbackStrength
        );

        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.Swing);
    }

    public float GetActionTimer() {
        return timedActionBehavior.GetActionTimer();
    }

    public void SetActionTimeRemaining(float timeRemaining) {
        timedActionBehavior.SetActionTimer(timeRemaining);
    }
}