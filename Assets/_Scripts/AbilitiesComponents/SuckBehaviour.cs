using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SuckBehaviour : MonoBehaviour {

    [SerializeField] private TriggerContactTracker contactTracker;
    [SerializeField] private float minForce;
    [SerializeField] private float maxForce;
    [SerializeField] private float suckSpeed;

    private float suckRadius;

    private StopMovement stopMovementEffect;

    [Header("Visual")]
    [SerializeField] private bool hasParticleField;
    [ConditionalHide("hasParticleField")][SerializeField] private ParticleSystemForceField particleField;

    [SerializeField] private bool emitParticles;
    [ConditionalHide("emitParticles")][SerializeField] private ParticleSystem particles;

    private void OnEnable() {
        stopMovementEffect = null;
    }

    private void OnDisable() {

        // stop preventing the object's movement
        if (stopMovementEffect != null) {
            Destroy(stopMovementEffect);
            stopMovementEffect = null;
        }
    }

    public void Setup(float suckRadius) {
        this.suckRadius = suckRadius;

        GetComponent<CircleCollider2D>().radius = suckRadius;

        // size visual based on area size
        if (emitParticles) {
            var main = particles.main;
            main.startLifetime = new ParticleSystem.MinMaxCurve(1, 0.3f * suckRadius);

            var emission = particles.emission;
            emission.rateOverTime = suckRadius * 10f;

            var shape = particles.shape;
            shape.radius = suckRadius;
        }

        if (hasParticleField) {
            particleField.endRange = suckRadius + 1;
        }
    }

    private void Update() {
        CheckTouchingEffectable();
    }

    private void FixedUpdate() {
        foreach (GameObject objectInRange in contactTracker.GetContacts()) {
            SuckObject(objectInRange);
        }
    }

    private void SuckObject(GameObject objectInRange) {
        Vector2 toSuckCenter = transform.position - objectInRange.transform.position;

        float distance = toSuckCenter.magnitude;

        float forceFactor = Mathf.Clamp(1 - (distance / suckRadius), 0, 1);
        float suckingForce = Mathf.Lerp(minForce, maxForce, forceFactor);

        // don't suck if very close to object to avoid jittering
        float distanceThreshold = 0.05f;
        bool touchingObject = distance < distanceThreshold;
        if (touchingObject) {
            suckingForce = 0;

            if (stopMovementEffect == null) {
                TryStopMovementOfTouching(objectInRange);
            }
        }

        Vector3 suckVelocity = toSuckCenter.normalized * suckingForce;

        if (objectInRange.TryGetComponent(out NavMeshAgent agent)) {
            agent.velocity = agent.desiredVelocity + suckVelocity;
        }
        else if (objectInRange.TryGetComponent(out Rigidbody2D rb)) {
            rb.velocity = Vector2.MoveTowards(rb.velocity, suckVelocity, suckSpeed * Time.fixedDeltaTime);
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
        bool touchingObject = distance < distanceThreshold;
        if (!touchingObject) {
            Destroy(stopMovementEffect);
            stopMovementEffect = null;
        }
    }
}
