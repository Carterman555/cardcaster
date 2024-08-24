using UnityEngine;

public class PlayerBasedMoveBehavior : EnemyBehavior {

    private Rigidbody2D rb;
    private Knockback knockback;

    private bool stopped;
    private bool chasing;
    private bool facingRight;

    public bool IsChasing() {
        return chasing;
    }

    public bool IsStopped() {
        return stopped;
    }

    public override void Initialize(Enemy enemy) {
        base.Initialize(enemy);
        
        if (enemy.TryGetComponent(out Rigidbody2D rigidbody2D)) {
            rb = rigidbody2D;
        }
        else {
            Debug.LogError("Object With Player Based Move Behavior Does Not Have Rigidbody2D!");
        }

        if (enemy.TryGetComponent(out Knockback knockback)) {
            this.knockback = knockback;
        }
        else {
            Debug.LogError("Object With Player Based Move Behavior Does Not Have Knockback!");
        }

        ChasePlayer();
        facingRight = true;
    }

    public void Start() {
        stopped = false;
    }
    public void Stop() {
        stopped = true;

        rb.velocity = Vector2.zero;
    }

    public void ChasePlayer() {
        chasing = true;
    }
    public void RunFromPlayer() {
        chasing = false;
    }

    public override void FrameUpdateLogic() {
        FaceTowardsPlayer();
    }

    public override void PhysicsUpdateLogic() {
        if (!stopped && !knockback.IsApplyingKnockback()) {
            Vector2 toPlayerDirection = (PlayerMovement.Instance.transform.position - enemy.transform.position).normalized;

            Vector2 moveDirection = toPlayerDirection;
            if (!chasing) {
                moveDirection = -toPlayerDirection;
            }

            rb.velocity = moveDirection * enemy.GetStats().MoveSpeed;
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
