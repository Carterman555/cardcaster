using UnityEngine;
using UnityEngine.AI;

public class FleePlayerBehavior : MonoBehaviour, IEffectable, IEnemyMovement {

    private IHasEnemyStats hasStats;
    private NavMeshAgent agent;
    private Knockback knockback;

    private bool movingToTargetPos;
    private Vector2 targetPosition;

    [SerializeField] private float searchDistance;
    [SerializeField] private float radius;

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

        if (movingToTargetPos) {
            float distanceToTargetThreshold = 1f;
            bool nearTargetPos = Vector2.Distance(transform.position, targetPosition) < distanceToTargetThreshold;
            if (nearTargetPos) {
                movingToTargetPos = false;
            }            
        }
        else {
            if (TryFindFleePosition()) {
                agent.SetDestination(targetPosition);
                movingToTargetPos = true;
            }
        }
    }

    private bool TryFindFleePosition() {
        Vector2 directionToPlayer = PlayerMovement.Instance.CenterPos - transform.position;
        Vector2 desiredEscapeDirection = -directionToPlayer.normalized;

        float degreeIncrement = 10f;
        int maxAttempts = Mathf.RoundToInt(180f / degreeIncrement);
        for (int i = 0; i < maxAttempts; i++) {

            float degrees = i * degreeIncrement;

            Vector3 escapeDirection1 = Quaternion.Euler(0, 0, degrees) * desiredEscapeDirection;
            Vector3 potentialEscapePosition1 = transform.position + escapeDirection1 * searchDistance;
            if (NavMesh.SamplePosition(potentialEscapePosition1, out NavMeshHit hit, radius, NavMesh.AllAreas)) {
                targetPosition = hit.position;
                return true;
            }

            Vector3 escapeDirection2 = Quaternion.Euler(0, 0, -degrees) * desiredEscapeDirection;
            Vector3 potentialEscapePosition2 = transform.position + escapeDirection2 * searchDistance;
            if (NavMesh.SamplePosition(potentialEscapePosition2, out NavMeshHit hit2, radius, NavMesh.AllAreas)) {
                targetPosition = hit2.position;
                return true;
            }
        }

        targetPosition = Vector3.zero;
        return false;
    }

    private void OnDisable() {

        // if disabled by enemy script, not from dying
        if (!GetComponent<EnemyHealth>().Dead && !Helpers.GameStopping()) {
            if (agent.isOnNavMesh) {
                agent.isStopped = true;
            }
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
