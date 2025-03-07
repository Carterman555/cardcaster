using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceOnContact : MonoBehaviour {

    [SerializeField] private LayerMask bounceLayer = GameLayers.ObstacleLayerMask | GameLayers.EnemyLayerMask;
    [SerializeField] private int maxBounces = 3;
    private int bounces;

    private Rigidbody2D rb;

    private Vector2 originalScale;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;
    }

    private void OnEnable() {
        bounces = 0;
        transform.localScale = originalScale;
    }

    private void OnTriggerEnter2D(Collider2D collision) {

        if (bounceLayer.ContainsLayer(collision.gameObject.layer)) {
            bool bouncesLeft = bounces < maxBounces;
            if (!bouncesLeft) {
                gameObject.ReturnToPool();
                return;
            }

            Bounce(collision);
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
    }
}
