using UnityEngine;

public class CircleSlashBehavior : MonoBehaviour {

    private TimedActionBehavior timedActionBehavior;
    [SerializeField] private Transform centerPoint;

    private Enemy enemy;

    [Header("Animation")]
    [SerializeField] private bool specialAttack;
    [SerializeField] private Animator anim;

    private void Awake() {
        enemy = GetComponent<Enemy>();

        timedActionBehavior = new TimedActionBehavior(
            enemy.EnemyStats.CommonStats.AttackCooldown,
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
            enemy.EnemyStats.AttackRange,
            enemy.EnemyStats.CommonStats.Damage,
            enemy.EnemyStats.CommonStats.KnockbackStrength
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