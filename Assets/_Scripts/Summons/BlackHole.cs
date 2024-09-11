using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BlackHole : MonoBehaviour {

    [SerializeField] private TriggerContactTracker contactTracker;
    [SerializeField] private float minForce;
    [SerializeField] private float maxForce;

    private float suckRadius;

    public void SetPosition(Vector3 position) {
        throw new System.NotImplementedException();
    }

    private void Awake() {
        suckRadius = GetComponent<CircleCollider2D>().radius;
    }

    private void FixedUpdate() {
        foreach (GameObject objectInRange in contactTracker.GetContacts()) {

            Vector2 toBlackHole = transform.position - objectInRange.transform.position;

            float distance = toBlackHole.magnitude;

            float forceFactor = Mathf.Clamp(1 - (distance / suckRadius), 0, 1);
            float suckingForce = Mathf.Lerp(minForce, maxForce, forceFactor);

            // don't suck if very close to black hole to avoid jittering
            float distanceThreshold = 0.05f;
            bool veryCloseToBlackHole = distance < distanceThreshold;
            if (veryCloseToBlackHole) {
                suckingForce = 0;
            }

            Vector3 suckVelocity = toBlackHole.normalized * suckingForce;

            if (objectInRange.TryGetComponent(out NavMeshAgent agent)) {

                agent.velocity = agent.desiredVelocity + suckVelocity;
            }
            else if (objectInRange.TryGetComponent(out Rigidbody2D rb)) {
                rb.velocity = suckVelocity;
            }
        }
    }

}
