using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FootstepAudio : MonoBehaviour {

    private Rigidbody2D rb;
    private NavMeshAgent agent;

    private bool wasMoving;
    private float stepTimer;

    [SerializeField] private AudioClips walkClips;
    [SerializeField] private float stepCooldown = 0.2f;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update() {

        // check if object is moved by rigidbody or agent and use the velocity to determine if moving
        bool moving = false;
        if (rb != null) {
            if (rb.velocity.magnitude > 0) { 
                moving = true;
            }
        }
        if (agent != null) {
            if (agent.velocity.magnitude > 0) {
                moving = true;
            }
        }

        bool startedMoving = !wasMoving && moving;
        wasMoving = moving;

        // play step sfx right as unit starts moving
        if (startedMoving) {
            stepTimer = 1f;
        }

        if (moving) {
            stepTimer += Time.deltaTime;
            if (stepTimer > stepCooldown) {
                AudioManager.Instance.PlaySound(walkClips);
                stepTimer = 0;
            }
        }
    }

}
