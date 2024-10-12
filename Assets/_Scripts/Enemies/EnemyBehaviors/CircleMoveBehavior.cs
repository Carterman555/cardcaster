using DG.Tweening;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.AI;

public class CircleMoveBehavior : EnemyBehavior, IMovementBehavior {

    private float moveRadius;

    private float angle;
    private Vector2 center;
    private NavMeshAgent agent;

    private ChangeFacingBehavior changeFacingBehavior;

    public CircleMoveBehavior(Enemy enemy, float radius) : base(enemy) {
        center = enemy.transform.position;

        changeFacingBehavior = new(enemy);

        if (enemy.TryGetComponent(out NavMeshAgent agent)) {
            this.agent = agent;
            agent.updateRotation = false;
            agent.updateUpAxis = false;
        }
        else {
            Debug.LogError("Object With CircleMoveBehavior Does Not Have NavMeshAgent!");
        }

        moveRadius = radius;
    }

    public override void OnEnable() {
        base.OnEnable();

        angle = 0f;

        // face right
        facingRight = true;
        enemy.transform.rotation = Quaternion.Euler(new Vector3(enemy.transform.rotation.eulerAngles.x, 0f, enemy.transform.rotation.eulerAngles.z));
        enemy.InvokeChangedFacing(facingRight);
    }

    public override void FrameUpdateLogic() {
        base.FrameUpdateLogic();

        if (IsStopped()) {
            return;
        }

        agent.speed = enemy.GetStats().MoveSpeed;

        float mult = 1 / moveRadius;
        angle += enemy.GetStats().MoveSpeed * mult * Time.deltaTime; // Increment angle based on speed
        float x = center.x + moveRadius * Mathf.Cos(angle);
        float y = center.y + moveRadius * Mathf.Sin(angle);
        Vector3 nextPosition = new Vector3(x, y);
        agent.SetDestination(nextPosition);

        bool faceRight = nextPosition.x > enemy.transform.position.x;
        HandleDirectionFacing(faceRight);
    }

    private bool facingRight;

    private void HandleDirectionFacing(bool faceRight) {
        if (!facingRight && faceRight) {
            enemy.transform.rotation = Quaternion.Euler(new Vector3(enemy.transform.rotation.eulerAngles.x, 0f, enemy.transform.rotation.eulerAngles.z));
            facingRight = true;
            enemy.InvokeChangedFacing(facingRight);
        }
        else if (facingRight && !faceRight) {
            enemy.transform.rotation = Quaternion.Euler(new Vector3(enemy.transform.rotation.eulerAngles.x, 180f, enemy.transform.rotation.eulerAngles.z));
            facingRight = false;
            enemy.InvokeChangedFacing(facingRight);
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

}
