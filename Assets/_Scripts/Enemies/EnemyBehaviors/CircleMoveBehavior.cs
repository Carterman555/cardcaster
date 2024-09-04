using UnityEngine;
using UnityEngine.AI;

public class CircleMoveBehavior : EnemyBehavior {

    private float moveRadius;

    private float angle;
    private Vector2 center;
    private NavMeshAgent agent;

    public override void Initialize(Enemy enemy) {
        base.Initialize(enemy);

        center = enemy.transform.position;

        if (enemy.TryGetComponent(out NavMeshAgent agent)) {
            this.agent = agent;
            agent.updateRotation = false;
            agent.updateUpAxis = false;
        }
        else {
            Debug.LogError("Object With CircleMoveBehavior Does Not Have NavMeshAgent!");
        }
    }

    public void Setup(float radius) {
        moveRadius = radius;
        angle = 0f;
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
