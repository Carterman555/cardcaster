using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.AI;

public class ChasePlayerBehavior : MonoBehaviour, IMovementBehavior {

    private IHasStats hasStats;
    private NavMeshAgent agent;
    private Knockback knockback;

    private void Awake() {

        // get components
        if (TryGetComponent(out IHasStats hasStats)) {
            this.hasStats = hasStats;
        }
        else {
            Debug.LogError("Object With ChasePlayerBehavior Does Not Have IHasStats!");
        }

        if (TryGetComponent(out NavMeshAgent agent)) {
            this.agent = agent;
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            agent.isStopped = false;
        }
        else {
            Debug.LogError("Object With ChasePlayerBehavior Does Not Have NavMeshAgent!");
        }

        if (TryGetComponent(out Knockback knockback)) {
            this.knockback = knockback;
        }
        else {
            Debug.LogError("Object With ChasePlayerBehavior Does Not Have Knockback!");
        }
    }

    private void Update() {
        agent.isStopped = false;
        agent.speed = hasStats.GetStats().MoveSpeed;
        agent.SetDestination(PlayerMovement.Instance.transform.position);
    }
}
