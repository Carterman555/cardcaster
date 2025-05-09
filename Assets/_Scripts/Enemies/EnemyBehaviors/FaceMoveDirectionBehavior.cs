using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FaceMoveDirectionBehavior : MonoBehaviour {

    [SerializeField] private bool flip;
    [SerializeField] private bool useAgent;

    private Rigidbody2D rb;
    private NavMeshAgent agent;

    private bool forcedToFace;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void OnEnable() {
        forcedToFace = false;
    }

    private void Update() {

        float xVel = useAgent ? agent.velocity.x : rb.velocity.x;

        if (xVel == 0 || forcedToFace) {
            return;
        }

        bool faceRight = xVel > 0;

        float facingRightRotation = flip ? 180 : 0;
        float facingLeftRotation = flip ? 0 : 180;

        float desiredRotation = faceRight ? facingRightRotation : facingLeftRotation;
        if (transform.rotation.eulerAngles.y != desiredRotation) {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, desiredRotation, transform.rotation.eulerAngles.z);
        }
    }

    public void ForceFace(bool faceRight) {
        forcedToFace = true;

        float facingRightRotation = flip ? 180 : 0;
        float facingLeftRotation = flip ? 0 : 180;

        float desiredRotation = faceRight ? facingRightRotation : facingLeftRotation;
        if (transform.rotation.eulerAngles.y != desiredRotation) {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, desiredRotation, transform.rotation.eulerAngles.z);
        }
    }

    public void StopForcing() {
        forcedToFace = false;
    }
}
