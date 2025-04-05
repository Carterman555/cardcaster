using System;
using UnityEngine;

public class BoomerangMovement : MonoBehaviour {

    public event Action OnReturn;

    private Vector2 launchDirection;
    private float speed;
    private float acceleration;

    private bool returning;

    public void Setup(Vector2 direction, float startingSpeed, float acceleration) {
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
            Vector2 playerPos = PlayerMovement.Instance.CenterPos;
            transform.position = Vector2.MoveTowards(transform.position, playerPos, speed * Time.fixedDeltaTime);

            speed += acceleration * Time.fixedDeltaTime;

            float distanceThreshold = 0.1f;
            if (Vector2.Distance(transform.position, playerPos) < distanceThreshold) {
                OnReturn?.Invoke();
                Destroy(this);
            }
        }
    }

}
