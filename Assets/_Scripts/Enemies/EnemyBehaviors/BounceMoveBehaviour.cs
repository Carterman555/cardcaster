using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BounceMoveBehaviour : MonoBehaviour {

    private Rigidbody2D rb;
    private IHasStats hasStats;

    [SerializeField] private TriggerContactTracker bounceTrigger;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        hasStats = GetComponent<IHasStats>();
    }

    private void OnEnable() {
        bounceTrigger.OnEnterContact += Bounce;

        RandomizeDirection();
        UpdateFacing();
    }
    private void OnDisable() {
        bounceTrigger.OnEnterContact -= Bounce;
    }

    private void RandomizeDirection() {
        Vector2 moveDirection = Vector2.up;

        float randomDegrees = Random.Range(0f, 360f);
        moveDirection = moveDirection.RotateDirection(randomDegrees);

        rb.velocity = moveDirection * hasStats.GetStats().MoveSpeed;
    }

    private void Bounce(GameObject collisionObject) {
        // Calculate the reflection vector
        Vector2 normal = collisionObject.GetComponent<Collider2D>().ClosestPoint(transform.position) - (Vector2)transform.position;
        Vector2 reflectDir = Vector2.Reflect(rb.velocity, normal.normalized); // Reflect based on current velocity

        // Set the new velocity
        rb.velocity = reflectDir.normalized * rb.velocity.magnitude; // Preserve the speed

        UpdateFacing();
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
}
