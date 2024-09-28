using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBlocker : MonoBehaviour {

    [SerializeField] private Animator anim;

    [SerializeField] private Vector3 topOffset;
    [SerializeField] private Vector3 bottomOffset;
    [SerializeField] private Vector3 leftOffset;
    [SerializeField] private Vector3 rightOffset;

    public void Setup(DoorwaySide side) {

        // remove after anim is setup (maybe)
        if (side == DoorwaySide.Top) {
            transform.position += topOffset;
        }
        else if (side == DoorwaySide.Bottom) {
            transform.position += bottomOffset;
        }
        else if (side == DoorwaySide.Left) {
            transform.position += leftOffset;
        }
        else if (side == DoorwaySide.Right) {
            transform.position += rightOffset;
        }
    }

    private void OnEnable() {
        CheckRoomCleared.OnEnemiesCleared += Open;
    }
    private void OnDisable() {
        CheckRoomCleared.OnEnemiesCleared -= Open;
    }

    private void Open() {
        anim.SetTrigger("open");
    }

    // played by anim
    private void ReturnToPool() {
        gameObject.ReturnToPool();
    }
}
