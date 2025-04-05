using UnityEngine;

public class PointTowardsPlayer : MonoBehaviour {

    [SerializeField] private float angleOffset;
    private Vector2 originalDirection;

    private void Awake() {
        originalDirection = transform.up;
    }

    void Update() {
        Vector2 toPlayer = PlayerMovement.Instance.CenterPos - transform.position;
        float toPlayerAngle = toPlayer.DirectionToRotation().eulerAngles.z;

        // mirror on left side
        bool onLeftSide = toPlayerAngle > 90 && toPlayerAngle < 270;
        if (onLeftSide) {
            toPlayerAngle = 180 - toPlayerAngle;
        }

        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, toPlayerAngle + angleOffset);
    }
}
