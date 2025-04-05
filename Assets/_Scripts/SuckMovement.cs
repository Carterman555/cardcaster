using System;
using UnityEngine;

public class SuckMovement : MonoBehaviour {

    public event Action OnReachTarget;

    private Transform target;

    [SerializeField] private float maxSpeed;
    [SerializeField] private float acceleration;

    private float speed;

    private Rigidbody2D rb;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Setup(Transform target) {
        this.target = target;
    }

    private void FixedUpdate() {
        if (target == null) {
            enabled = false;
            return;
        }

        Suck();
    }

    private void Suck() {
        Vector2 toSuckCenter = target.position - transform.position;

        rb.velocity = speed * toSuckCenter.normalized;

        speed = Mathf.MoveTowards(speed, maxSpeed, acceleration * Time.fixedDeltaTime);

        // don't suck if very close to object to avoid jittering
        float distanceThreshold = 0.2f;
        float distance = toSuckCenter.magnitude;
        bool touchingObject = distance < distanceThreshold;
        if (touchingObject) {
            target = null;
            enabled = false;

            rb.velocity = Vector2.zero;

            OnReachTarget?.Invoke();
        }
    }
}
