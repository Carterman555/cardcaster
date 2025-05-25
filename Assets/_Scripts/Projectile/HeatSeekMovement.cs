using UnityEngine;

public class HeatSeekMovement : MonoBehaviour, ITargetProjectileMovement {

    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private float rotationSpeed = 2;

    private Transform target;
    private Rigidbody2D rb;

    [SerializeField] private bool rotateObject;

    // I couldn't figure out how to get the consistent rotation I wanted without using transform.up
    // to track the direction
    private Transform directionTracker;

    public GameObject GetObject() {
        return gameObject;
    }

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable() {
        if (directionTracker == null) {
            directionTracker = new GameObject("Direction Tracker").transform;
            directionTracker.SetParent(transform);
            directionTracker.transform.position = transform.position;
        }
    }

    public void Setup(Transform target) {
        this.target = target;

        Transform directionTransform = rotateObject ? transform : directionTracker;

        Vector2 toTarget = target.position - transform.position;
        directionTransform.up = toTarget;
    }

    private void FixedUpdate() {

        Transform directionTransform = rotateObject ? transform : directionTracker;

        // move
        rb.velocity = directionTransform.up * moveSpeed;

        // rotate
        Vector2 toTarget = target.position - transform.position;
        directionTransform.up = Vector3.MoveTowards(directionTransform.up, toTarget, rotationSpeed * Time.fixedDeltaTime);
    }
}
