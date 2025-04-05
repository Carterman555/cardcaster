using UnityEngine;
using UnityEngine.AI;

public class ChasePlayerBehavior : MonoBehaviour, IEffectable, IEnemyMovement {

    private IHasEnemyStats hasStats;
    private NavMeshAgent agent;
    private Knockback knockback;

    private void Awake() {

        hasStats = GetComponent<IHasEnemyStats>();

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        knockback = GetComponent<Knockback>();
    }

    private void Update() {

        if (!IsMoving()) {
            return;
        }

        agent.isStopped = false;
        agent.speed = hasStats.EnemyStats.MoveSpeed;
        agent.SetDestination(PlayerMovement.Instance.CenterPos);
    }

    private void OnEnable() {
        agent.isStopped = false; // moved from awake: might cause bugs
    }

    private void OnDisable() {

        if (Helpers.GameStopping() || GameSceneManager.Instance.IsSceneLoading()) {
            return;
        }

        // if disabled by enemy script, not from dying
        if (!GetComponent<EnemyHealth>().Dead) {
            agent.isStopped = true;
        }
    }

    public void OnAddEffect(UnitEffect unitEffect) {
        if (unitEffect is StopMovement) {
            agent.isStopped = true;
        }
    }

    public bool IsMoving() {
        bool hasStopEffect = TryGetComponent(out StopMovement stopMovement);
        return !hasStopEffect && !knockback.IsApplyingKnockback() && enabled;
    }
}
