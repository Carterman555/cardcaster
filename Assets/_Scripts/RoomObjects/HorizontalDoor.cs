using UnityEngine;

public class HorizontalDoor : MonoBehaviour {

    [SerializeField] private Animator anim;

    [SerializeField] private TriggerContactTracker topPlayerTrigger;
    [SerializeField] private TriggerContactTracker bottomPlayerTrigger;

    private void OnEnable() {
        topPlayerTrigger.OnEnterContact += OpenDownwards;
        bottomPlayerTrigger.OnEnterContact += OpenUpwards;
    }
    private void OnDisable() {
        topPlayerTrigger.OnEnterContact -= OpenDownwards;
        bottomPlayerTrigger.OnEnterContact -= OpenUpwards;
    }

    private void OpenDownwards(GameObject player) {
        anim.SetTrigger("openDown");
    }
    private void OpenUpwards(GameObject player) {
        anim.SetTrigger("openUp");
    }
}
