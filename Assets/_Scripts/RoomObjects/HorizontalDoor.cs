using UnityEngine;
using static System.Runtime.CompilerServices.RuntimeHelpers;

public class HorizontalDoor : MonoBehaviour {

    [SerializeField] private Animator anim;

    [SerializeField] private TriggerContactTracker topPlayerTrigger;
    [SerializeField] private TriggerContactTracker bottomPlayerTrigger;

    [SerializeField] private TriggerContactTracker cardBlockerTrigger;

    private bool hasBeenOpenedDown;
    private bool hasBeenOpenedUp;

    private bool doorIsBlocked;

    private void OnEnable() {
        topPlayerTrigger.OnEnterContact += TryOpenDownwards;
        bottomPlayerTrigger.OnEnterContact += TryOpenUpwards;

        cardBlockerTrigger.OnEnterContact += TryClose;
        cardBlockerTrigger.OnExitContact += TryReopen;
    }
    private void OnDisable() {
        topPlayerTrigger.OnEnterContact -= TryOpenDownwards;
        bottomPlayerTrigger.OnEnterContact -= TryOpenUpwards;

        cardBlockerTrigger.OnEnterContact -= TryClose;
        cardBlockerTrigger.OnExitContact -= TryReopen;
    }

    private void TryOpenDownwards(GameObject player) {
        if (!doorIsBlocked) {
            anim.SetTrigger("openDown");
        }
    }
    private void TryOpenUpwards(GameObject player) {
        if (!doorIsBlocked) {
            anim.SetTrigger("openUp");
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
