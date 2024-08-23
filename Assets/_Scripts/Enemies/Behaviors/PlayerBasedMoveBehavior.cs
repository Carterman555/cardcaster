using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerBasedMoveBehavior : EnemyBehavior {

    private float moveSpeed;

    private Rigidbody2D rb;

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
            Debug.LogError("Object With Chase Player Behavior Does Not Have Rigidbody2D!");
        }

        ChasePlayer();
        facingRight = true;
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
        if (!stopped) {
            Vector2 toPlayerDirection = (PlayerMovement.Instance.transform.position - enemy.transform.position).normalized;

            Vector2 moveDirection = toPlayerDirection;
            if (!chasing) {
                moveDirection = -toPlayerDirection;
            }

            rb.velocity = moveDirection * moveSpeed;
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
