using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBlocker : MonoBehaviour {

    [SerializeField] private Animator anim;

    public void Setup(bool horizontal) {

        if (horizontal) {
            //anim.SetTrigger("horizontalClose");
        }
        else {
            //anim.SetTrigger("verticalClose");
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
