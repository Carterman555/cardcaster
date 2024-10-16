using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatSeekMovement : MonoBehaviour, ITargetProjectileMovement {

    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotationSpeed;

    private Transform target;
    private Rigidbody2D rb;
    private IDelayedReturn[] delayedReturns;

    private bool returning;

    public GameObject GetObject() {
        return gameObject;
    }

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        delayedReturns = GetComponents<IDelayedReturn>();
    }

    public void Setup(Transform target) {
        this.target = target;

        Vector2 toTarget = target.position - transform.position;
        transform.up = toTarget;
    }

    private void FixedUpdate() {

        if (returning) {
            return;
        }

        // move
        rb.velocity = transform.up * moveSpeed;

        // rotate
        Vector2 toTarget = target.position - transform.position;
        transform.up = Vector3.MoveTowards(transform.up, toTarget, rotationSpeed * Time.fixedDeltaTime);
    }

    // whenever a behavior starts to return this object (the death animation starts), stop it from dealing damage
    private void OnEnable() {
        foreach (var delayedReturn in delayedReturns) {
            delayedReturn.OnStartReturn += SetReturning;
        }
    }
    private void OnDisable() {
        foreach (var delayedReturn in delayedReturns) {
            delayedReturn.OnStartReturn -= SetReturning;
        }
    }

    public void SetReturning() {
        returning = false;
    }
}
