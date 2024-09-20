using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;

public class BlackHole : MonoBehaviour {

    [SerializeField] private TriggerContactTracker contactTracker;
    [SerializeField] private float minForce;
    [SerializeField] private float maxForce;
    [SerializeField] private float duration;
    [SerializeField] private float suckSpeed;

    private float durationTimer;

    private float suckRadius;

    private GameObject effectableTouching;
    private bool stoppedMovement;

    private bool dying;

    private void Awake() {
        suckRadius = GetComponent<CircleCollider2D>().radius;
    }

    private void OnEnable() {
        effectableTouching = null;
        stoppedMovement = false;
        durationTimer = 0;
        dying = false;
    }

    private void Update() {

        if (dying) {
            return;
        }

        CheckTouchingEffectable();

        durationTimer += Time.deltaTime;
        if (durationTimer > duration) {
            durationTimer = 0;

            if (effectableTouching != null) {
                effectableTouching.GetComponent<IEffectable>().RemoveEffect(new StopMovement());
                stoppedMovement = false;
                effectableTouching = null;
            }

            dying = true;

            transform.ShrinkThenDestroy();
        }
    }

    private void FixedUpdate() {

        if (dying) {
            return;
        }

        foreach (GameObject objectInRange in contactTracker.GetContacts()) {

            Vector2 toBlackHole = transform.position - objectInRange.transform.position;

            float distance = toBlackHole.magnitude;

            float forceFactor = Mathf.Clamp(1 - (distance / suckRadius), 0, 1);
            float suckingForce = Mathf.Lerp(minForce, maxForce, forceFactor);

            // don't suck if very close to black hole to avoid jittering
            float distanceThreshold = 0.05f;
            bool touchingBlackHole = distance < distanceThreshold;
            if (touchingBlackHole) {
                suckingForce = 0;

                if (effectableTouching == null) {
                    TryStopMovementOfTouching(objectInRange);
                }
            }

            Vector3 suckVelocity = toBlackHole.normalized * suckingForce;

            if (objectInRange.TryGetComponent(out NavMeshAgent agent)) {
                agent.velocity = agent.desiredVelocity + suckVelocity;
            }
            else if (objectInRange.TryGetComponent(out Rigidbody2D rb)) {
                rb.velocity = Vector2.MoveTowards(rb.velocity, suckVelocity, suckSpeed * Time.fixedDeltaTime);
            }
        }
    }

    private void TryStopMovementOfTouching(GameObject objectTouching) {
        if (objectTouching.TryGetComponent(out IEffectable effectable)) {
            effectableTouching = objectTouching;

            if (!stoppedMovement) {
                stoppedMovement = true;
                effectable.AddEffect(new StopMovement());
            }
        }
    }

    private void CheckTouchingEffectable() {

        if (effectableTouching == null) {
            return;
        }

        float distanceThreshold = 0.05f;
        float distance = Vector2.Distance(effectableTouching.transform.position, transform.position);
        bool touchingBlackHole = distance < distanceThreshold;
        if (!effectableTouching) {
            effectableTouching.GetComponent<IEffectable>().RemoveEffect(new StopMovement());
            stoppedMovement = false;
            effectableTouching = null;
        }
    }
}
