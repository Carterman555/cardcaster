using UnityEngine;

public class VerticalDoor : MonoBehaviour {

    [SerializeField] private Animator anim;

    [SerializeField] private TriggerContactTracker leftPlayerTrigger;
    [SerializeField] private TriggerContactTracker rightPlayerTrigger;

    private void OnEnable() {
        leftPlayerTrigger.OnEnterContact += OpenRight;
        rightPlayerTrigger.OnEnterContact += OpenLeft;
    }
    private void OnDisable() {
        leftPlayerTrigger.OnEnterContact -= OpenRight;
        rightPlayerTrigger.OnEnterContact -= OpenLeft;
    }

    private void OpenRight(GameObject player) {
        anim.SetTrigger("openRight");
    }
    private void OpenLeft(GameObject player) {
        anim.SetTrigger("openLeft");
    }
}
