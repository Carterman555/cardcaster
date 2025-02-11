using UnityEngine;

public class StraightMovement : MonoBehaviour {

    [SerializeField] private float moveSpeed;

    private Rigidbody2D rb;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Setup(Vector2 direction) {
        transform.up = direction.normalized;
        rb.velocity = direction.normalized * moveSpeed;
    }

    public void Setup(Vector2 direction, float moveSpeed) {
        transform.up = direction.normalized;
        this.moveSpeed = moveSpeed;
        rb.velocity = direction.normalized * moveSpeed;
    }
}
