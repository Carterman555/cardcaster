using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceOnContact : MonoBehaviour, IDelayedReturn {

    public event Action OnStartReturn;

    [SerializeField] private LayerMask bounceLayer;
    [SerializeField] private int maxBounces = 3;
    private int bounces;

    private Rigidbody2D rb;

    private bool returning;

    private Vector2 originalScale;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;

    }

    private void OnEnable() {
        bounces = 0;
        returning = false;
        transform.localScale = originalScale;
    }

    private void OnTriggerEnter2D(Collider2D collision) {

        bool bouncesLeft = bounces < maxBounces;
        if (!bouncesLeft && !returning) {
            transform.ShrinkThenDestroy();
            OnStartReturn?.Invoke();
            return;
        }

        if (bounceLayer.ContainsLayer(collision.gameObject.layer)) {
            Bounce(collision);
        }
    }

    private void Bounce(Collider2D collision) {
        // Calculate the reflection vector
        Vector2 normal = collision.ClosestPoint(transform.position) - (Vector2)transform.position;
        Vector2 reflectDir = Vector2.Reflect(rb.velocity, normal.normalized); // Reflect based on current velocity

        // Set the new velocity
        rb.velocity = reflectDir.normalized * rb.velocity.magnitude; // Preserve the speed

        bounces++;
    }
}
