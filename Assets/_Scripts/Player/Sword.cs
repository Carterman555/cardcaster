using DG.Tweening;
using System;
using UnityEngine;

public class Sword : MonoBehaviour {

    [SerializeField] private float minRotation;
    [SerializeField] private float maxRotation;

    private bool facingRight = true;
    private bool inUpPos = true;
    private bool swinging = false;

    private float targetRotation;

    [SerializeField] private float swingAcceleration;

    [SerializeField] private float afterSwingRotation;
    [SerializeField] private float afterSwingRotateDuration;
    [SerializeField] private float afterSwingRotateBackDuration;

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
        CalculateTargetAngle();
    }

    /// <summary>
    /// calculates the target angle of the sword based on the mouse position and if the sword is in the up or down position
    /// </summary>
    private void CalculateTargetAngle() {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 toMouseDirection = mousePosition - transform.position;

        float mouseAngle = toMouseDirection.DirectionToRotation().eulerAngles.z;

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

        // rotate 180 degrees if in down position
        if (!inUpPos) {
            targetRotation -= 180;
        }
    }



    private void FixedUpdate() {

        // if the sword is not swinging, smoothly rotate it towards target rotation
        if (!swinging) {
            Quaternion targetQuaternion = Quaternion.Normalize(Quaternion.Euler(0f, 0f, targetRotation));
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetQuaternion, swingAcceleration * Time.fixedDeltaTime);
        }
    }

    private void Swing() {
        swinging = true;
        inUpPos = !inUpPos;

        transform.DOKill();

        // rotate sword to opposite side (180 degrees) from target angle
        transform.localRotation = Quaternion.Euler(0f, 0f, targetRotation);
        transform.Rotate(new Vector3(0f, 0f, 180f));

        // the sword rotates after swing because of the momentum from the swing

        // calculate the rotation to swing to
        float afterSwingTargetRotation;
        if (inUpPos) {
            afterSwingTargetRotation = transform.localRotation.eulerAngles.z + afterSwingRotation;
        }
        else {
            afterSwingTargetRotation = transform.localRotation.eulerAngles.z - afterSwingRotation;
        }

        Vector3 originalRotation = transform.localRotation.eulerAngles;
        transform.DOLocalRotate(new Vector3(0, 0, afterSwingTargetRotation), afterSwingRotateDuration).SetEase(Ease.OutSine).OnComplete(() => {

            // then the sword rotates back to target rotation
            transform.DOLocalRotate(originalRotation, afterSwingRotateBackDuration).SetEase(Ease.InOutSine).OnComplete(() => {
                swinging = false;
            });
        });
    }
}
