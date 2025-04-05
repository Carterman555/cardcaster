using UnityEngine;

public class HeatSeekMovement : MonoBehaviour, ITargetProjectileMovement {

    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotationSpeed;

    private Transform target;
    private Rigidbody2D rb;

    public GameObject GetObject() {
        return gameObject;
    }

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Setup(Transform target) {
        this.target = target;

        Vector2 toTarget = target.position - transform.position;
        transform.up = toTarget;
    }

    private void FixedUpdate() {

        // move
        rb.velocity = transform.up * moveSpeed;

        // rotate
        Vector2 toTarget = target.position - transform.position;
        transform.up = Vector3.MoveTowards(transform.up, toTarget, rotationSpeed * Time.fixedDeltaTime);
    }
}
