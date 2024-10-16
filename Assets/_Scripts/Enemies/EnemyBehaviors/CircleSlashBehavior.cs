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
            hasStats.GetStats().AttackCooldown,
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

    private void Attack() {
        DamageDealer.DealCircleDamage(
            GameLayers.PlayerLayerMask,
            centerPoint.position,
            hasStats.GetEnemyStats().AttackRange,
            hasStats.GetStats().Damage,
            hasStats.GetStats().KnockbackStrength
        );
    }
}