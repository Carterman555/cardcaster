using UnityEngine;

public class HeatSeekMovement : MonoBehaviour, ITargetProjectileMovement {

    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private float rotationSpeed = 2;

    private Transform target;
    private Rigidbody2D rb;

    [SerializeField] private bool rotateObject;

    public GameObject GetObject() {
        return gameObject;
    }

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Setup(Transform target) {
        this.target = target;
    }

    private void FixedUpdate() {

        Vector2 toTarget = target.position - transform.position;
        rb.velocity = Vector3.MoveTowards(rb.velocity, toTarget, rotationSpeed * Time.fixedDeltaTime);

        if (rotateObject) {
            transform.up = rb.velocity;
        }
    }
}
