using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class WeaponRotate : MonoBehaviour {

    [SerializeField] private float minRotation;
    [SerializeField] private float maxRotation;

    private bool facingRight = true;
    private bool inUpPos = true;
    private bool swinging = false;

    private float targetRotation;

    [SerializeField] private float swingAcceleration;
    private float swingMomentum;

    private void OnEnable() {
        PlayerMovement.OnChangedFacing += PlayerChangedFacing;
        PlayerMeleeAttack.OnAttack += Swing;

        inUpPos = true;
    }
    private void OnDisable() {
        PlayerMovement.OnChangedFacing -= PlayerChangedFacing;
        PlayerMeleeAttack.OnAttack -= Swing;
    }

    private void PlayerChangedFacing(bool facingRight) {
        this.facingRight = facingRight;

        inUpPos = true;
    }

    private void Update() {

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 toMouseDirection = mousePosition - transform.position;

        float mouseAngle = Mathf.Atan2(toMouseDirection.y, toMouseDirection.x) * Mathf.Rad2Deg;

        // Convert the angle from -180 to 180 range to 0 to 360 range
        if (mouseAngle < 0) {
            mouseAngle += 360;
        }

        // rotate the angle 90 degrees, so down is 0
        mouseAngle = (mouseAngle + 90) % 360;


        // change the lerp points based on the way the player is facing so normalizedMouseAngle ranges from 0 to 1
        // depending on how high the mouse angle is. (1 = mouse above player, 0 = mouse below player)
        float normalizedMouseAngle = 0;
        if (facingRight) {
            normalizedMouseAngle = Mathf.InverseLerp(0f, 180f, mouseAngle); // between 0 and 1
        }
        else {
            normalizedMouseAngle = Mathf.InverseLerp(360f, 180f, mouseAngle); // between 0 and 1
        }

        targetRotation = Mathf.Lerp(minRotation, maxRotation, normalizedMouseAngle);

        if (!inUpPos) {
            targetRotation -= 180;
        }
    }

    [SerializeField] private float afterSwingRotation;
    [SerializeField] private float afterSwingSpeed;
    private float afterSwingTargetRotation;

    private void FixedUpdate() {

        if (!swinging) {
            Quaternion targetQuaternion = Quaternion.Normalize(Quaternion.Euler(0f, 0f, targetRotation));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetQuaternion, swingAcceleration * Time.fixedDeltaTime);
        }
        else {
            Quaternion targetQuaternion = Quaternion.Normalize(Quaternion.Euler(0f, 0f, afterSwingTargetRotation));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetQuaternion, swingAcceleration * Time.fixedDeltaTime);

            float angleDistanceToTarget = Mathf.Abs(transform.rotation.eulerAngles.z - afterSwingTargetRotation);
            if (angleDistanceToTarget < 1f) {
                swinging = false;
            }
        }
    }

    private void Swing() {
        swinging = true;
        inUpPos = !inUpPos;

        transform.Rotate(new Vector3(0f, 0f, 180f));

        if (inUpPos) {
            afterSwingTargetRotation = transform.rotation.eulerAngles.z + afterSwingRotation;
        }
        else {
            afterSwingTargetRotation = transform.rotation.eulerAngles.z - afterSwingRotation;
        }
    }
}
