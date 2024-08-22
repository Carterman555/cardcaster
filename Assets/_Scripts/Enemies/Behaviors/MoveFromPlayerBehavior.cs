using UnityEngine;

public class MoveFromPlayerBehavior : EnemyBehavior {

    private float moveSpeed;

    private Rigidbody2D rb;

    private bool stopped;

    public override void Initialize(Enemy enemy) {
        base.Initialize(enemy);

        if (enemy.TryGetComponent(out Rigidbody2D rigidbody2D)) {
            rb = rigidbody2D;
        }
        else {
            Debug.LogError("Object With MoveFromPlayerBehavior Does Not Have Rigidbody2D!");
        }
    }

    public void SetSpeed(float speed) {
        moveSpeed = speed;
    }

    public void Start() {
        stopped = false;
    }
    public void Stop() {
        stopped = true;

        rb.velocity = Vector2.zero;
    }

    public override void PhysicsUpdateLogic() {
        if (!stopped) {
            Vector2 toPlayerDirection = (PlayerMovement.Instance.transform.position - enemy.transform.position).normalized;
            rb.velocity = -toPlayerDirection * moveSpeed;
        }
    }
}
