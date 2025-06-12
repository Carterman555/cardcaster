using System;
using UnityEngine;

public class FacePlayerBehavior : MonoBehaviour {

    public event Action<bool> OnChangedFacing;

    public bool FacingRight { get; private set; }

    private void OnEnable() {

        // face right
        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, 0f, transform.rotation.eulerAngles.z));
        FacingRight = true;
        OnChangedFacing?.Invoke(FacingRight);
    }

    private void Update() {
        FaceTowardsPosition(PlayerMovement.Instance.CenterPos.x);
    }

    private void FaceTowardsPosition(float xPos) {
        float playerXPos = PlayerMovement.Instance.CenterPos.x;

        bool playerToRight = playerXPos > transform.position.x;

        if (!FacingRight && playerToRight) {
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, 0f, transform.rotation.eulerAngles.z));
            FacingRight = true;
            OnChangedFacing?.Invoke(FacingRight);
        }
        else if (FacingRight && !playerToRight) {
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, 180f, transform.rotation.eulerAngles.z));
            FacingRight = false;
            OnChangedFacing?.Invoke(FacingRight);
        }
    }

    // set facingRight to opposite of what is should to make sure FaceTowardsPosition updates the facing direction
    public void UpdateFacing(float xPos) {
        FacingRight = xPos < transform.position.x;
    }
}
