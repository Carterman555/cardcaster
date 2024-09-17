using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeFacingBehavior : EnemyBehavior {

    private bool facingRight;

    public ChangeFacingBehavior(Enemy enemy) : base(enemy) {
        facingRight = true;
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

}
