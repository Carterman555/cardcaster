using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeFacingBehavior : MonoBehaviour {

    public static event Action<bool> OnChangedFacing;

    private bool facingRight;

    private void OnEnable() {

        // face right
        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, 0f, transform.rotation.eulerAngles.z));
        facingRight = true;
        OnChangedFacing?.Invoke(facingRight);
    }

    public void FaceTowardsPosition(float xPos) {
        float playerXPos = PlayerMovement.Instance.transform.position.x;

        bool mouseToRight = playerXPos > transform.position.x;

        if (!facingRight && mouseToRight) {
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, 0f, transform.rotation.eulerAngles.z));
            facingRight = true;
            OnChangedFacing?.Invoke(facingRight);
        }
        else if (facingRight && !mouseToRight) {
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
