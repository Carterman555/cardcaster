using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.AI;

public class ChasePlayerBehavior : MonoBehaviour, IEffectable, IEnemyMovement {

    private IHasStats hasStats;
    private NavMeshAgent agent;
    private Knockback knockback;

    private void Awake() {

        hasStats = GetComponent<IHasStats>();

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.isStopped = false;

        knockback = GetComponent<Knockback>();
    }

    private void Update() {

        if (!IsMoving()) {
            return;
        }

        agent.isStopped = false;
        agent.speed = hasStats.GetStats().MoveSpeed;
        agent.SetDestination(PlayerMovement.Instance.transform.position);
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
