using UnityEngine;

public class VerticalDoor : MonoBehaviour {

    [SerializeField] private Animator anim;

    [SerializeField] private TriggerContactTracker leftPlayerTrigger;
    [SerializeField] private TriggerContactTracker rightPlayerTrigger;

    [SerializeField] private TriggerContactTracker cardBlockerTrigger;

    private bool hasBeenOpenedLeft;
    private bool hasBeenOpenedRight;

    private bool doorIsBlocked;

    private bool HasBeenOpened => hasBeenOpenedLeft || hasBeenOpenedRight;

    private void OnEnable() {
        leftPlayerTrigger.OnEnterContact_GO += TryOpenRight;
        rightPlayerTrigger.OnEnterContact_GO += TryOpenLeft;

        cardBlockerTrigger.OnEnterContact_GO += TryClose;
        cardBlockerTrigger.OnExitContact_GO += TryReopen;

        hasBeenOpenedLeft = false;
        hasBeenOpenedRight = false;
        doorIsBlocked = false;
    }
    private void OnDisable() {
        leftPlayerTrigger.OnEnterContact_GO -= TryOpenRight;
        rightPlayerTrigger.OnEnterContact_GO -= TryOpenLeft;

        cardBlockerTrigger.OnEnterContact_GO -= TryClose;
        cardBlockerTrigger.OnExitContact_GO -= TryReopen;
    }

    private void TryOpenRight(GameObject player) {
        if (!doorIsBlocked && !HasBeenOpened) {
            anim.ResetTrigger("close");
            anim.SetTrigger("openRight");
            hasBeenOpenedRight = true;
        }
    }
    private void TryOpenLeft(GameObject player) {
        if (!doorIsBlocked && !HasBeenOpened) {
            anim.ResetTrigger("close");
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
