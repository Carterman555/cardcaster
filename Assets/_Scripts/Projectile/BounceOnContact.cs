using UnityEngine;

public class BounceOnContact : MonoBehaviour {

    public LayerMask BounceLayerMask;
    [SerializeField] private int maxBounces = 3;
    private int bounces;

    private Rigidbody2D rb;

    private Vector2 originalScale;

    [SerializeField] private bool useRaycast;

    [SerializeField] private bool playBounceSfx;
    [SerializeField, ConditionalHide("playBounceSfx")] private AudioClips bounceSfx;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;
    }

    private void OnEnable() {
        bounces = 0;
        transform.localScale = originalScale;
    }

    private void Update() {
        if (!useRaycast) {
            return;
        }

        if (BounceLayerMask == 0) {
            Debug.LogWarning("BounceLayerMask not set!");
        }

        float checkDistance = 0.5f;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, rb.velocity.normalized, checkDistance, BounceLayerMask);
        if (hit) {
            bool bouncesLeft = bounces < maxBounces;
            if (bouncesLeft) {
                Bounce(hit.collider);
            }
            else {
                gameObject.TryReturnToPool();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {

        if (useRaycast) {
            return;
        }

        if (BounceLayerMask == 0) {
            Debug.LogWarning("BounceLayerMask not set!");
        }

        if (BounceLayerMask.ContainsLayer(collision.gameObject.layer)) {
            bool bouncesLeft = bounces < maxBounces;
            if (bouncesLeft) {
                Bounce(collision);
            }
            else {
                gameObject.TryReturnToPool();
            }
        }
    }

    private void Bounce(Collider2D collision) {
        // Calculate the reflection vector
        Vector2 normal = collision.ClosestPoint(transform.position) - (Vector2)transform.position;
        Vector2 reflectDir = Vector2.Reflect(rb.velocity, normal.normalized); // Reflect based on current velocity

        // Set the new velocity
        rb.velocity = reflectDir.normalized * rb.velocity.magnitude; // Preserve the speed

        // rotate the projectile that way
        transform.up = reflectDir.normalized;

        bounces++;

        if (playBounceSfx) {
            AudioManager.Instance.PlaySingleSound(bounceSfx);
        }
    }
}
