using UnityEngine;

public class StraightMovement : MonoBehaviour {

    [SerializeField] private float moveSpeed;

    private Rigidbody2D rb;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Setup(Vector2 direction, bool updateUpDirection = true) {
        rb.velocity = direction.normalized * moveSpeed;

        if (updateUpDirection) {
            transform.up = direction.normalized;
        }
    }

    public void Setup(Vector2 direction, float moveSpeed, bool updateUpDirection = true) {
        this.moveSpeed = moveSpeed;
        rb.velocity = direction.normalized * moveSpeed;

        if (updateUpDirection) {
            transform.up = direction.normalized;
        }
    }
}
