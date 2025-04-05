using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Localization.SmartFormat.Utilities;

public class Smasher : MonoBehaviour {

    private Vector2[] directions = { Vector2.down, Vector2.up, Vector2.left, Vector2.right };
    private Vector2 lastDirection;
    private Vector2 currentDirection;

    [SerializeField] private float acceleration;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float smashCooldown;
    private float smashTimer;

    private bool smashing;

    [SerializeField] private BoxCollider2D col;
    private Rigidbody2D rb;

    [SerializeField] private bool debugDirection;
    [SerializeField] private Vector2[] directionOrder;
    private int directionIndex;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable() {
        ResetSmash();
        directionIndex = 0;
        currentDirection = directionOrder[directionIndex];
    }

    private void Update() {
        if (!smashing) {
            smashTimer += Time.deltaTime;
            if (smashTimer > smashCooldown) {
                Smash();
                rb.velocity = currentDirection * maxSpeed;
            }
        }
    }

    private void FixedUpdate() {
        if (smashing) {
            // rb.velocity = Mathf.MoveTowards(rb.velocity.magnitude, maxSpeed, acceleration * Time.fixedDeltaTime) * currentDirection;
        }
    }

    private void Smash() {
        smashing = true;
    }

    private void OnCollisionEnter2D(Collision2D collision) {

        if (smashing && GameLayers.ObstacleLayerMask.ContainsLayer(collision.gameObject.layer)) {
            ResetSmash();
        }
    }

    private void ResetSmash() {
        smashing = false;
        smashTimer = 0;

        rb.velocity = Vector2.zero;

        //Vector2 moveAmount = -currentDirection * 0.1f;
        //rb.MovePosition(rb.position + moveAmount);

        lastDirection = currentDirection;

        List<Vector2> validDirections = directions.Where(d => d != lastDirection).ToList();

        // don't choose direction that where the smasher an obstacle in that direction
        for (int i = validDirections.Count - 1; i >= 0; i--) {

            Vector2 boxPos = (Vector2)transform.position + col.offset + (validDirections[i] * 0.2f);
            if (Physics2D.OverlapBox(boxPos, col.size, 0f, GameLayers.ObstacleLayerMask)) {
                validDirections.RemoveAt(i);
            }
        }

        currentDirection = validDirections.RandomItem();

        if (debugDirection) {
            directionIndex++;
            currentDirection = directionOrder[directionIndex];
        }
    }

    private void OnDrawGizmos() {

        foreach (Vector2 direction in directions) {
            Vector2 pos = (Vector2)transform.position + col.offset + (direction * 0.2f);
            Gizmos.DrawWireCube(pos, col.size);
        }
    }
}
