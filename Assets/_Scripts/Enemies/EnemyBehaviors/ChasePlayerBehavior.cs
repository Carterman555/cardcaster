using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.AI;

public class ChasePlayerBehavior : EnemyBehavior {

    private ChangeFacingBehavior changeFacingBehavior;

    private NavMeshAgent agent;
    private Knockback knockback;

    public override void Initialize(Enemy enemy) {
        base.Initialize(enemy);

        // initialize behaviors
        changeFacingBehavior = new();
        changeFacingBehavior.Initialize(enemy);

        // get components
        if (enemy.TryGetComponent(out NavMeshAgent agent)) {
            this.agent = agent;
            agent.updateRotation = false;
            agent.updateUpAxis = false;
        }
        else {
            Debug.LogError("Object With ChasePlayerBehavior Does Not Have NavMeshAgent!");
        }

        if (enemy.TryGetComponent(out Knockback knockback)) {
            this.knockback = knockback;
        }
        else {
            Debug.LogError("Object With ChasePlayerBehavior Does Not Have Knockback!");
        }
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
            agent.SetDestination(PlayerMovement.Instance.transform.position);
        }
    }
}
