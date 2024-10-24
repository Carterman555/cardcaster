using DG.Tweening.Core.Easing;
using Mono.CSharp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Knockback))]
public class BounceMoveBehaviour : MonoBehaviour, IEffectable, IEnemyMovement {

    private Rigidbody2D rb;
    private Knockback knockback;
    private IHasStats hasStats;

    private Vector2 velocity;

    [SerializeField] private TriggerContactTracker bounceTrigger;

    [SerializeField] private Transform centerPoint;

    [SerializeField] private bool hasBounceVariation;
    [ConditionalHide("hasBounceVariation")][SerializeField] private float bounceVariation;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        knockback = GetComponent<Knockback>();
        hasStats = GetComponent<IHasStats>();
    }

    private void OnEnable() {
        bounceTrigger.OnEnterContact += Bounce;

        RandomizeDirection();
        UpdateFacing();
    }
    private void OnDisable() {
        bounceTrigger.OnEnterContact -= Bounce;

        rb.velocity = Vector2.zero;
    }

    private void RandomizeDirection() {
        velocity = Vector2.up * hasStats.GetStats().MoveSpeed;

        float randomDegrees = Random.Range(0f, 360f);
        velocity = velocity.RotateDirection(randomDegrees);
    }

    private void Update() {

        if (!IsMoving()) {
            return;
        }

        rb.velocity = velocity;
    }

    private void Bounce(GameObject collisionObject) {
        // Calculate the reflection vector
        Vector2 normal = collisionObject.GetComponent<Collider2D>().ClosestPoint(centerPoint.position) - (Vector2)centerPoint.position;
        Vector2 reflectDir = Vector2.Reflect(velocity, normal.normalized); // Reflect based on current velocity

        if (hasBounceVariation) {
            float randomAngle = Random.Range(-bounceVariation, bounceVariation);
            reflectDir = reflectDir.RotateDirection(randomAngle);
        }

        // Set the new velocity
        velocity = reflectDir.normalized * velocity.magnitude; // Preserve the speed

        UpdateFacing();

        print("bounce");
    }

    private void UpdateFacing() {
        // if going right
        if (rb.velocity.x > 0f) {
            FaceRight();
        }
        // if going left
        else if (rb.velocity.x < 0f) {
            FaceLeft();
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
