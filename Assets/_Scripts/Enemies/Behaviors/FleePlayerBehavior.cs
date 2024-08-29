using UnityEngine;
using UnityEngine.AI;

public class FleePlayerBehavior : EnemyBehavior {

    private NavMeshAgent agent;
    private Knockback knockback;

    private bool facingRight;

    public override void Initialize(Enemy enemy) {
        base.Initialize(enemy);

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

        facingRight = true;
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
        FaceTowardsPlayer();

        agent.speed = enemy.GetStats().MoveSpeed;
    }

    public override void PhysicsUpdateLogic() {
        if (!IsStopped() && !knockback.IsApplyingKnockback()) {
            agent.SetDestination(PlayerMovement.Instance.transform.position);
        }
    }

    private void FaceTowardsPlayer() {
        float playerXPos = PlayerMovement.Instance.transform.position.x;

        bool mouseToRight = playerXPos > enemy.transform.position.x;

        if (!facingRight && mouseToRight) {
            enemy.transform.rotation = Quaternion.Euler(new Vector3(enemy.transform.rotation.eulerAngles.x, 0f, enemy.transform.rotation.eulerAngles.z));
            facingRight = true;
            enemy.InvokeChangedFacing(facingRight);
        }
        else if (facingRight && !mouseToRight) {
            enemy.transform.rotation = Quaternion.Euler(new Vector3(enemy.transform.rotation.eulerAngles.x, 180f, enemy.transform.rotation.eulerAngles.z));
            facingRight = false;
            enemy.InvokeChangedFacing(facingRight);
        }
    }


}
