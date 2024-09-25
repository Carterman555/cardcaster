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
    [SerializeField] private float suckSpeed;

    private float damage;
    private float duration;

    private float suckRadius;
    private bool dying;

    private StopMovement stopMovementEffect;

    private void Awake() {
        suckRadius = GetComponent<CircleCollider2D>().radius;
    }

    private void OnEnable() {
        stopMovementEffect = null;
        dying = false;
    }

    public void SetAbilityStats(AbilityStats stats) {
        damage = stats.Damage;
        duration = stats.Duration;
    }

    private void Update() {

        if (dying) {
            return;
        }

        CheckTouchingEffectable();

        duration -= Time.deltaTime;
        if (duration < 0) {
            if (stopMovementEffect != null) {
                Destroy(stopMovementEffect);
                stopMovementEffect = null;
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

                if (stopMovementEffect == null) {
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
            if (stopMovementEffect == null) {
                stopMovementEffect = objectTouching.AddComponent<StopMovement>();
                stopMovementEffect.Setup();
            }
        }
    }

    private void CheckTouchingEffectable() {

        if (stopMovementEffect == null) {
            return;
        }

        float distanceThreshold = 0.05f;
        float distance = Vector2.Distance(stopMovementEffect.transform.position, transform.position);
        bool touchingBlackHole = distance < distanceThreshold;
        if (!touchingBlackHole) {
            Destroy(stopMovementEffect);
            stopMovementEffect = null;
        }
    }
}
