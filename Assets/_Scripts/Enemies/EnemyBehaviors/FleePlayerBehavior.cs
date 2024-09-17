using UnityEngine;
using UnityEngine.AI;

public class FleePlayerBehavior : EnemyBehavior, IMovementBehavior {

    private ChangeFacingBehavior changeFacingBehavior;

    private NavMeshAgent agent;
    private Knockback knockback;

    public FleePlayerBehavior(Enemy enemy) : base(enemy) {
        // initialize behaviors
        changeFacingBehavior = new(enemy);

        // get components
        if (enemy.TryGetComponent(out NavMeshAgent agent)) {
            this.agent = agent;
            agent.updateRotation = false;
            agent.updateUpAxis = false;
        }
        else {
            Debug.LogError("Object With FleePlayerBehavior Does Not Have NavMeshAgent!");
        }

        if (enemy.TryGetComponent(out Knockback knockback)) {
            this.knockback = knockback;
        }
        else {
            Debug.LogError("Object With FleePlayerBehavior Does Not Have Knockback!");
        }

        Stop();
    }

    public override void Start() {
        base.Start();
        agent.isStopped = false;
    }
    public override void Stop() {
        base.Stop();
        agent.isStopped = true;
    }

    public override void FrameUpdateLogic() {
        if (IsStopped()) {
            return;
        }

        changeFacingBehavior.FaceTowardsPosition(PlayerMovement.Instance.transform.position.x);

        agent.isStopped = false;
        agent.speed = enemy.GetStats().MoveSpeed;
    }

    public override void PhysicsUpdateLogic() {
        if (!IsStopped() && !knockback.IsApplyingKnockback()) {
            TryEscapeFromPlayer();
        }
    }

    private void TryEscapeFromPlayer() {
        Vector3 directionToPlayer = PlayerMovement.Instance.transform.position - enemy.transform.position;
        Vector3 desiredEscapeDirection = -directionToPlayer.normalized;

        int maxAttempts = 10;
        for (int i = 0; i < maxAttempts; i++) {

            float runAwayDistance = 10f;

            // Calculate potential escape position
            Vector3 potentialEscapePosition = enemy.transform.position + desiredEscapeDirection * runAwayDistance;

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
}
