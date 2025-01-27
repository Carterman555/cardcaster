using DG.Tweening.Core.Easing;
using Mono.CSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Knockback))]
public class BounceMoveBehaviour : MonoBehaviour, IEffectable, IEnemyMovement {

    public event Action OnBounce;

    private Rigidbody2D rb;
    private Knockback knockback;
    private IHasStats hasStats;

    private Vector2 velocity;

    private float emergencyBounceTimer;

    [SerializeField] private TriggerContactTracker bounceTrigger;

    [SerializeField] private Transform centerPoint;

    [SerializeField] private bool hasBounceVariation;
    [ConditionalHide("hasBounceVariation")][SerializeField] private float bounceVariation;

    [SerializeField] private bool twoWayFacing = true;
    [ConditionalHideReversed("twoWayFacing")] [SerializeField] private float facingAngleOffset;

    [SerializeField] private bool hasSFX;
    [ConditionalHide("hasSFX")] [SerializeField] private AudioClips bounceSFX;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        knockback = GetComponent<Knockback>();
        hasStats = GetComponent<IHasStats>();
    }

    private void OnEnable() {
        bounceTrigger.OnEnterContact_GO += Bounce;

        RandomizeDirection();

        emergencyBounceTimer = 0;
    }
    private void OnDisable() {
        bounceTrigger.OnEnterContact_GO -= Bounce;

        // reset direction facing
        if (!twoWayFacing) {
            transform.rotation = Quaternion.identity;
        }

        rb.velocity = Vector2.zero;
    }

    private void RandomizeDirection() {
        velocity = Vector2.up * hasStats.GetStats().MoveSpeed;

        float randomDegrees = UnityEngine.Random.Range(0f, 360f);
        velocity.RotateDirection(randomDegrees);

        UpdateFacing(velocity);
    }

    private void Update() {

        if (!IsMoving()) {
            return;
        }

        // if the enemy is touching an obstacle for a certain amount of time then a
        // glitch has occured and try to bounce away from the wall
        if (bounceTrigger.HasContact()) {
            emergencyBounceTimer += Time.deltaTime;
            float delayBeforeEmergencyBounce = 0.1f;
            if (emergencyBounceTimer > delayBeforeEmergencyBounce) {
                Bounce(bounceTrigger.GetFirstContact());
            }
        }
        else {
            emergencyBounceTimer = 0;
        }

        rb.velocity = velocity;
    }

    private void Bounce(GameObject collisionObject) {

        // Calculate the reflection vector
        Vector2 normal = collisionObject.GetComponent<Collider2D>().ClosestPoint(centerPoint.position) - (Vector2)centerPoint.position;
        Vector2 reflectDir = Vector2.Reflect(velocity, normal.normalized); // Reflect based on current velocity

        // A safe guard because sometimes the bounce glitches and enemies bounce twice at the same time, causing the
        // enemy to bounce back into the wall.
        bool verticalBounce = Mathf.Abs(normal.y) > Mathf.Abs(normal.x);
        if (verticalBounce) {
            bool bounceIntoWall = Mathf.Sign(normal.y) == Mathf.Sign(reflectDir.y);
            if (bounceIntoWall) {
                return;
            }
        }
        else {
            bool bounceIntoWall = Mathf.Sign(normal.x) == Mathf.Sign(reflectDir.x);
            if (bounceIntoWall) {
                return;
            }
        }

        // Set the new velocity
        velocity = reflectDir.normalized * velocity.magnitude; // Preserve the speed

        UpdateFacing(velocity);

        if (hasSFX) {
            AudioManager.Instance.PlaySound(bounceSFX);
        }

        OnBounce?.Invoke();
    }

    private void UpdateFacing(Vector2 velocity) {

        if (twoWayFacing) {
            // if going right
            if (velocity.x > 0f) {
                FaceRight();
            }
            // if going left
            else if (velocity.x < 0f) {
                FaceLeft();
            }
        }
        else {
            Vector2 faceDirection = velocity.normalized;
            faceDirection.RotateDirection(facingAngleOffset);
            transform.up = faceDirection;
        }
    }

    private void FaceRight() {
        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, 0f, transform.rotation.eulerAngles.z));
    }

    private void FaceLeft() {
        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, 180f, transform.rotation.eulerAngles.z));
    }

    public void OnAddEffect(UnitEffect unitEffect) {
        if (unitEffect is StopMovement) {
            rb.velocity = Vector2.zero;
        }
    }

    public bool IsMoving() {
        bool hasStopEffect = TryGetComponent(out StopMovement stopMovement);
        return !hasStopEffect && !knockback.IsApplyingKnockback() && enabled;
    }
}
