using UnityEngine;

public class VerticalDoor : MonoBehaviour {

    [SerializeField] private Animator anim;

    [SerializeField] private TriggerContactTracker leftPlayerTrigger;
    [SerializeField] private TriggerContactTracker rightPlayerTrigger;

    [SerializeField] private TriggerContactTracker cardBlockerTrigger;

    private bool hasBeenOpenedLeft;
    private bool hasBeenOpenedRight;

    private bool doorIsBlocked;

    private void OnEnable() {
        leftPlayerTrigger.OnEnterContact += TryOpenRight;
        rightPlayerTrigger.OnEnterContact += TryOpenLeft;

        cardBlockerTrigger.OnEnterContact += TryClose;
        cardBlockerTrigger.OnExitContact += TryReopen;

        hasBeenOpenedLeft = false;
        hasBeenOpenedRight = false;
        doorIsBlocked = false;
    }
    private void OnDisable() {
        leftPlayerTrigger.OnEnterContact -= TryOpenRight;
        rightPlayerTrigger.OnEnterContact -= TryOpenLeft;

        cardBlockerTrigger.OnEnterContact -= TryClose;
        cardBlockerTrigger.OnExitContact -= TryReopen;
    }

    private void TryOpenRight(GameObject player) {
        if (!doorIsBlocked) {
            anim.SetTrigger("openRight");
            hasBeenOpenedRight = true;
        }
    }
    private void TryOpenLeft(GameObject player) {
        if (!doorIsBlocked) {
            anim.SetTrigger("openLeft");
            hasBeenOpenedLeft = true;
        }
    }

    private void TryClose(GameObject roomObject) {
        if (roomObject.TryGetComponent(out DoorBlocker doorBlocker)) {
            anim.SetTrigger("close");
            anim.ResetTrigger("openRight");
            anim.ResetTrigger("openLeft");
            doorIsBlocked = true;
        }
    }

    private void TryReopen(GameObject roomObject) {
        if (roomObject.TryGetComponent(out DoorBlocker doorBlocker)) {
            if (hasBeenOpenedLeft) {
                anim.ResetTrigger("openRight");
                anim.SetTrigger("openLeft");
            }
            else if (hasBeenOpenedRight) {
                anim.ResetTrigger("openLeft");
                anim.SetTrigger("openRight");
            }
            doorIsBlocked = false;
        }
    }
}
