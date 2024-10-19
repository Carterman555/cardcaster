using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class FleePlayerBehavior : MonoBehaviour, IEffectable, IEnemyMovement {

    private IHasStats hasStats;
    private NavMeshAgent agent;
    private Knockback knockback;

    private void Awake() {
        hasStats = GetComponent<IHasStats>();

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
        agent.speed = hasStats.GetStats().MoveSpeed;
        TryEscapeFromPlayer();
    }

    private void TryEscapeFromPlayer() {
        Vector3 directionToPlayer = PlayerMovement.Instance.transform.position - transform.position;
        Vector3 desiredEscapeDirection = -directionToPlayer.normalized;

        int maxAttempts = 10;
        for (int i = 0; i < maxAttempts; i++) {

            float runAwayDistance = 10f;

            // Calculate potential escape position
            Vector3 potentialEscapePosition = transform.position + desiredEscapeDirection * runAwayDistance;

            // Check if the position is on the NavMesh
            NavMeshHit hit;
            if (NavMesh.SamplePosition(potentialEscapePosition, out hit, runAwayDistance, NavMesh.AllAreas)) {
                // Valid position found, set it as the destination
                agent.SetDestination(hit.position);
                return;
            }

            // If we reach here, the position wasn't valid. Adjust the direction slightly and try again.
            desiredEscapeDirection = Quaternion.Euler(0, Random.Range(-45f, 45f), 0) * desiredEscapeDirection;
        }

        // If we've exhausted all attempts, just don't move
        Debug.LogWarning("Couldn't find a valid escape position");
    }

    private void OnDisable() {
        if (!GetComponent<Health>().IsDead()) {
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
