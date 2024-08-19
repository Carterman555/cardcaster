using UnityEngine;

public class WeaponRotate : MonoBehaviour {

    [SerializeField] private float minRotation;
    [SerializeField] private float maxRotation;

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
        bool mouseRightOfPlayer = mouseAngle < 180;
        float normalizedMouseAngle = 0;
        if (mouseRightOfPlayer) {
            normalizedMouseAngle = Mathf.InverseLerp(0f, 180f, mouseAngle); // between 0 and 1
        }
        else {
            normalizedMouseAngle = Mathf.InverseLerp(360f, 180f, mouseAngle); // between 0 and 1
        }

        float weaponRotation = Mathf.Lerp(minRotation, maxRotation, normalizedMouseAngle);
        transform.localRotation = Quaternion.Euler(0f, 0f, weaponRotation);
    }
}
