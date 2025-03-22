using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.AI;

public class ChasePlayerBehavior : MonoBehaviour, IEffectable, IEnemyMovement {

    private IHasCommonStats hasStats;
    private NavMeshAgent agent;
    private Knockback knockback;

    private void Awake() {

        hasStats = GetComponent<IHasCommonStats>();

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
        agent.speed = hasStats.CommonStats.MoveSpeed;
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
        if (!GetComponent<Health>().Dead) {
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
