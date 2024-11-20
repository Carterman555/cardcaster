using UnityEngine;
using static System.Runtime.CompilerServices.RuntimeHelpers;

public class HorizontalDoor : MonoBehaviour {

    [SerializeField] private Animator anim;

    [SerializeField] private TriggerContactTracker topPlayerTrigger;
    [SerializeField] private TriggerContactTracker bottomPlayerTrigger;

    [SerializeField] private TriggerContactTracker cardBlockerTrigger;

    private bool HasBeenOpened => hasBeenOpenedDown || hasBeenOpenedUp;
    private bool hasBeenOpenedDown;
    private bool hasBeenOpenedUp;

    private bool doorIsBlocked;

    private void OnEnable() {
        topPlayerTrigger.OnEnterContact_GO += TryOpenDownwards;
        bottomPlayerTrigger.OnEnterContact_GO += TryOpenUpwards;

        cardBlockerTrigger.OnEnterContact_GO += TryClose;
        cardBlockerTrigger.OnExitContact_GO += TryReopen;
    }
    private void OnDisable() {
        topPlayerTrigger.OnEnterContact_GO -= TryOpenDownwards;
        bottomPlayerTrigger.OnEnterContact_GO -= TryOpenUpwards;

        cardBlockerTrigger.OnEnterContact_GO -= TryClose;
        cardBlockerTrigger.OnExitContact_GO -= TryReopen;
    }

    private void TryOpenDownwards(GameObject player) {
        if (!doorIsBlocked && !HasBeenOpened) {
            anim.ResetTrigger("close");
            anim.SetTrigger("openDown");
            hasBeenOpenedDown = true;
        }
    }
    private void TryOpenUpwards(GameObject player) {
        if (!doorIsBlocked && !HasBeenOpened) {
            anim.ResetTrigger("close");
            anim.SetTrigger("openUp");
            hasBeenOpenedUp = true;
        }
    }

    private void TryClose(GameObject roomObject) {
        if (roomObject.TryGetComponent(out DoorBlocker doorBlocker)) {
            anim.SetTrigger("close");
            anim.ResetTrigger("openDown");
            anim.ResetTrigger("openUp");
            doorIsBlocked = true;
        }
    }

    private void TryReopen(GameObject roomObject) {
        if (roomObject.TryGetComponent(out DoorBlocker doorBlocker)) {
            if (hasBeenOpenedDown) {
                anim.ResetTrigger("openUp");
                anim.SetTrigger("openDown");
            }
            else if (hasBeenOpenedUp) {
                anim.ResetTrigger("openDown");
                anim.SetTrigger("openUp");
            }
            doorIsBlocked = false;
        }
    }
}
