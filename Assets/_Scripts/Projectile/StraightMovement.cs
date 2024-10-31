using UnityEngine;

public class StraightMovement : MonoBehaviour {

    [SerializeField] private float moveSpeed;

    private Rigidbody2D rb;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Setup(Vector2 direction) {
        transform.up = direction;
        rb.velocity = direction * moveSpeed;
    }

    public void Setup(Vector2 direction, float moveSpeed) {
        transform.up = direction;
        this.moveSpeed = moveSpeed;
        rb.velocity = direction * moveSpeed;
    }
}
