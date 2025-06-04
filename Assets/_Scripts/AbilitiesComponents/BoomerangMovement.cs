using System;
using UnityEngine;

public class BoomerangMovement : MonoBehaviour {

    public event Action<BoomerangMovement> OnReturn;

    private Transform returnTarget;
    private Vector2 launchDirection;
    private float speed;
    private float acceleration;

    private bool returning;

    public void Setup(Transform returnTarget, Vector2 direction, float startingSpeed, float acceleration) {
        this.returnTarget = returnTarget;
        launchDirection = direction.normalized;
        speed = startingSpeed;
        this.acceleration = acceleration;

        returning = false;
    }

    private void FixedUpdate() {
        if (!returning) {
            transform.position += (Vector3)launchDirection * speed * Time.fixedDeltaTime;

            speed -= acceleration * Time.fixedDeltaTime;
            if (speed < 0) {
                speed = 0;
                returning = true;
            }
        }
        else {
            transform.position = Vector2.MoveTowards(transform.position, returnTarget.position, speed * Time.fixedDeltaTime);

            speed += acceleration * Time.fixedDeltaTime;

            float distanceThreshold = 0.1f;
            if (Vector2.Distance(transform.position, returnTarget.position) < distanceThreshold) {
                OnReturn?.Invoke(this);
            }
        }
    }

}
