using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeFacingBehavior : EnemyBehavior {

    private bool facingRight;

    public ChangeFacingBehavior(Enemy enemy) : base(enemy) {
    }

    public override void OnEnable() {
        base.OnEnable();

        // face right
        enemy.transform.rotation = Quaternion.Euler(new Vector3(enemy.transform.rotation.eulerAngles.x, 0f, enemy.transform.rotation.eulerAngles.z));
        facingRight = true;
        enemy.InvokeChangedFacing(facingRight);
    }

    public void FaceTowardsPosition(float xPos) {
        float playerXPos = PlayerMovement.Instance.transform.position.x;

        bool mouseToRight = playerXPos > enemy.transform.position.x;

        if (!facingRight && mouseToRight) {
            enemy.transform.rotation = Quaternion.Euler(new Vector3(enemy.transform.rotation.eulerAngles.x, 0f, enemy.transform.rotation.eulerAngles.z));
            facingRight = true;
            enemy.InvokeChangedFacing(facingRight);
        }
        else if (facingRight && !mouseToRight) {
            enemy.transform.rotation = Quaternion.Euler(new Vector3(enemy.transform.rotation.eulerAngles.x, 180f, enemy.transform.rotation.eulerAngles.z));
            facingRight = false;
            enemy.InvokeChangedFacing(facingRight);
        }
    }

    // set facingRight to opposite of what is should to make sure FaceTowardsPosition updates the facing direction
    public void UpdateFacing(float xPos) {
        facingRight = xPos < enemy.transform.position.x;
    }
}
