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
    [SerializeField] private RandomFloat smashCooldown;
    [Tooltip("If moving slowly for this amount of time, change directions")]
    [SerializeField] private float slowMovingTime;

    private float slowMovingTimer;
    private float smashTimer;
    private bool smashing;

    [SerializeField] private BoxCollider2D col;
    private Rigidbody2D rb;

    [Header("Visual")]
    [SerializeField] private ParticleSystem collideParticles;
    [SerializeField] private ParticleSystem sideCollideParticles;
    private float speedBeforeCollision;

    [SerializeField] private ParticleSystem trailParticles;

    [SerializeField] private GameObject arrow;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable() {
        ResetSmash();
    }

    private void Update() {
        
        if (rb.velocity.magnitude > 0) {
            speedBeforeCollision = rb.velocity.magnitude;
        }

        if (!smashing) {
            rb.velocity = Vector2.zero;

            smashTimer += Time.deltaTime;
            if (smashTimer > smashCooldown.Value) {
                StartSmashing();
            }
        }
        else if (smashing) {
            float slowMovingThreshold = 0.5f;

            bool hitObstacle = rb.velocity.magnitude == 0;
            if (hitObstacle) {
                OnSmash();
                ResetSmash();
            }
            else if (rb.velocity.magnitude < slowMovingThreshold) {
                slowMovingTimer += Time.deltaTime;
                if (slowMovingTimer > slowMovingTime) {
                    ResetSmash();
                }
            }
            else {
                slowMovingTimer = 0;
            }
        }
    }

    private void FixedUpdate() {
        if (smashing) {
            rb.velocity = Mathf.MoveTowards(rb.velocity.magnitude, maxSpeed, acceleration * Time.fixedDeltaTime) * currentDirection;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (smashing && collision.gameObject.TryGetComponent(out Smasher smasher)) {
            OnSmash();
            ResetSmash();
        }
    }

    public void StartSmashing() {
        smashing = true;

        // make sure vel is not 0, so doesn't register as hit something right away
        rb.velocity = Mathf.MoveTowards(rb.velocity.magnitude, maxSpeed, acceleration * Time.fixedDeltaTime) * currentDirection;

        trailParticles.Play();
        arrow.SetActive(false);
    }

    private void ResetSmash() {
        smashing = false;
        smashCooldown.Randomize();
        smashTimer = 0;
        slowMovingTimer = 0;

        rb.velocity = Vector2.zero;

        lastDirection = currentDirection;
        currentDirection = GetRandomValidDirection();

        SetTrailParticlePosition();
        trailParticles.Stop();

        arrow.SetActive(true);
        arrow.transform.up = currentDirection;
    }

    private Vector2 GetRandomValidDirection() {
        List<Vector2> validDirections = directions.Where(d => d != lastDirection).ToList();

        // don't choose direction that where the smasher an obstacle in that direction
        for (int i = validDirections.Count - 1; i >= 0; i--) {

            Vector2 boxPos = (Vector2)transform.position + (col.offset * transform.localScale) + (validDirections[i] * 0.2f);
            Collider2D[] cols = Physics2D.OverlapBoxAll(boxPos, col.size * transform.localScale * 0.98f, 0f, GameLayers.ObstacleLayerMask);
            if (cols.Any(c => c.gameObject != gameObject)) {
                validDirections.RemoveAt(i);
            }
        }

        if (validDirections.Count == 0) {
            return Vector2.zero;
        }

        return validDirections.RandomItem();
    }

    private void OnSmash() {

        // collide particles
        float collideBurstCountMult = 1.5f;
        SetBurstCount(collideParticles, (int)(speedBeforeCollision * collideBurstCountMult));
        collideParticles.Play();

        // side collide particles
        if (currentDirection == Vector2.up) {
            sideCollideParticles.transform.SetLocalPositionAndRotation(new(0, 3.3f), Quaternion.identity);
        }
        else if (currentDirection == Vector2.down) {
            sideCollideParticles.transform.SetLocalPositionAndRotation(new(0, 0.3f), Quaternion.identity);
        }
        else if (currentDirection == Vector2.left) {
            sideCollideParticles.transform.SetLocalPositionAndRotation(new(-1.5f, 2f), Quaternion.Euler(0, 0, 90));
        }
        else if (currentDirection == Vector2.right) {
            sideCollideParticles.transform.SetLocalPositionAndRotation(new(1.5f, 2f), Quaternion.Euler(0, 0, 90));
        }

        float sideCollideBurstCountMult = 1.5f;
        SetBurstCount(sideCollideParticles, (int)(speedBeforeCollision * sideCollideBurstCountMult));

        Vector2 boxPos = (Vector2)transform.position + (col.offset * transform.localScale) + (currentDirection * 0.2f);
        bool obstacleAhead = Physics2D.OverlapBox(boxPos, col.size * transform.localScale * 0.98f, 0f, GameLayers.ObstacleLayerMask);
        if (obstacleAhead) {
            sideCollideParticles.Play();
        }

        // camera shake
        float cameraShakeMult = 0.025f;
        CameraShaker.Instance.ShakeCamera(speedBeforeCollision * cameraShakeMult);
    }

    private void SetTrailParticlePosition() {
        if (currentDirection == Vector2.up) {
            trailParticles.transform.SetLocalPositionAndRotation(new(0, 3.3f), Quaternion.identity);
        }
        else if (currentDirection == Vector2.down) {
            trailParticles.transform.SetLocalPositionAndRotation(new(0, 3.3f), Quaternion.identity);
        }
        else if (currentDirection == Vector2.left) {
            trailParticles.transform.SetLocalPositionAndRotation(new(1.5f, 2f), Quaternion.Euler(0, 0, 90));
        }
        else if (currentDirection == Vector2.right) {
            trailParticles.transform.SetLocalPositionAndRotation(new(-1.5f, 2f), Quaternion.Euler(0, 0, 90));
        }
    }

    private void SetBurstCount(ParticleSystem particles, int count) {
        var emission = particles.emission;
        var burst = emission.GetBurst(0);
        burst.count = count;
        emission.SetBurst(0, burst);
    }

    private void OnDrawGizmos() {

        foreach (Vector2 direction in directions) {
            Vector2 pos = (Vector2)transform.position + (col.offset * transform.localScale) + (direction * 0.2f);
            Gizmos.DrawWireCube(pos, col.size * transform.localScale * 0.98f);
        }

        float length = 5f;
        Gizmos.DrawLine(transform.position, transform.position + ((Vector3)currentDirection * length));
    }
}
