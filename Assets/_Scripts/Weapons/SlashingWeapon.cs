using DG.Tweening;
using UnityEngine;

public class SlashingWeapon : MonoBehaviour {

    private bool facingRight = true;
    private bool inUpPos = true;
    private bool swinging = false;

    private float targetRotation;

    private Transform target;

    [SerializeField] private float swingAcceleration;
    [SerializeField] private float afterSwingRotation;

    private IChangesFacing changesFacing;

    private void Awake() {
        changesFacing = GetComponentInParent<IChangesFacing>();
    }

    private void OnEnable() {
        changesFacing.OnChangedFacing += ChangeFacing;
    }
    private void OnDisable() {
        changesFacing.OnChangedFacing -= ChangeFacing;
    }

    public void ChangeFacing(bool facingRight) {
        this.facingRight = facingRight;

        inUpPos = true;
    }

    public void SetTarget(Transform target) {
        this.target = target;
    }

    private void Update() {
        CalculateTargetAngle();
    }

    /// <summary>
    /// calculates the target angle of the sword based on the target position and if the sword is in the up or down position
    /// </summary>
    private void CalculateTargetAngle() {
        Vector2 toTargetDirection = target.position - transform.position;

        float targetAngle = toTargetDirection.DirectionToRotation().eulerAngles.z;

        // Convert the angle from -180 to 180 range to 0 to 360 range
        if (targetAngle < 0) {
            targetAngle += 360;
        }

        // rotate the angle 90 degrees, so down is 0
        targetAngle = (targetAngle + 90) % 360;

        // change the lerp points based on the way the player is facing so normalizedTargetAngle ranges from 0 to 1
        // depending on how high the target angle is. (1 = target above player, 0 = target below player)
        float normalizedTargetAngle = 0;
        if (facingRight) {
            normalizedTargetAngle = Mathf.InverseLerp(0f, 180f, targetAngle); // between 0 and 1
        }
        else {
            normalizedTargetAngle = Mathf.InverseLerp(360f, 180f, targetAngle); // between 0 and 1
        }

        float minRotation = -30f;
        float maxRotation = 100f;
        targetRotation = Mathf.Lerp(minRotation, maxRotation, normalizedTargetAngle);

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

    public void Swing() {
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
        float afterSwingRotateDuration = 0.1f;
        transform.DOLocalRotate(new Vector3(0, 0, afterSwingTargetRotation), afterSwingRotateDuration).SetEase(Ease.OutSine).OnComplete(() => {

            // then the sword rotates back to target rotation
            float afterSwingRotateBackDuration = 0.5f;
            transform.DOLocalRotate(originalRotation, afterSwingRotateBackDuration).SetEase(Ease.InOutSine).OnComplete(() => {
                swinging = false;
            });
        });
    }


}
