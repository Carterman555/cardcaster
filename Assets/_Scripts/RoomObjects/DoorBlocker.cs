using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBlocker : MonoBehaviour {

    [SerializeField] private Animator anim;

    public void Setup(DoorwaySide side) {

        bool horizontal = side == DoorwaySide.Top || side == DoorwaySide.Bottom;

        if (horizontal) {
            //anim.SetTrigger("horizontalClose");
        }
        else {
            //anim.SetTrigger("verticalClose");
        }

        // remove after anim is setup (maybe)
        if (side == DoorwaySide.Top) {
            transform.position -= new Vector3(0, 2f);
        }
    }

    private void OnEnable() {
        Enemy.OnEnemiesCleared += Open;
    }
    private void OnDisable() {
        Enemy.OnEnemiesCleared -= Open;
    }

    private void Open() {
        //anim.SetTrigger("Open");
        ReturnToPool(); // remove after anim is setup
    }

    // played by anim
    private void ReturnToPool() {
        gameObject.ReturnToPool();
    }
}
