using System;
using UnityEngine;

public class FacePlayerBehavior : MonoBehaviour {

    public event Action<bool> OnChangedFacing;

    private bool facingRight;

    private void OnEnable() {

        // face right
        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, 0f, transform.rotation.eulerAngles.z));
        facingRight = true;
        OnChangedFacing?.Invoke(facingRight);
    }

    private void Update() {
        FaceTowardsPosition(PlayerMovement.Instance.CenterPos.x);
    }

    private void FaceTowardsPosition(float xPos) {
        float playerXPos = PlayerMovement.Instance.CenterPos.x;

        bool playerToRight = playerXPos > transform.position.x;

        if (!facingRight && playerToRight) {
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, 0f, transform.rotation.eulerAngles.z));
            facingRight = true;
            OnChangedFacing?.Invoke(facingRight);
        }
        else if (facingRight && !playerToRight) {
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, 180f, transform.rotation.eulerAngles.z));
            facingRight = false;
            OnChangedFacing?.Invoke(facingRight);
        }
    }

    // set facingRight to opposite of what is should to make sure FaceTowardsPosition updates the facing direction
    public void UpdateFacing(float xPos) {
        facingRight = xPos < transform.position.x;
    }
}
